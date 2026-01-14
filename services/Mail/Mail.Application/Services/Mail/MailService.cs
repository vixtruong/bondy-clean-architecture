using Bondy.Contracts.Dtos.Mail;
using Bondy.SharedKernel.Abstractions;
using Bondy.SharedKernel.Application;
using Bondy.SharedKernel.Common;
using Bondy.SharedKernel.Constants;
using Mail.Application.Abstractions.Repositories;
using Mail.Application.Abstractions.Templating;
using Mail.Application.Mapper;
using Mail.Application.Templating;
using Mail.Domain.Entities;
using Mail.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Mail.Application.Services.Mail;

public sealed class MailService : ApplicationServiceBase, IMailService
{
    #region Constructor

    private readonly IMailRepository _mail;
    private readonly ITemplateRenderer _renderer;
    private readonly ITemplateProvider _provider;
    private readonly IEmailSender _sender;

    public MailService(ILogger<MailService> logger, IClock clock, IMailRepository mail, ITemplateRenderer mailRenderer, ITemplateProvider mailProvider, IEmailSender sender) : base(logger, clock)
    {
        _mail = mail;
        _renderer = mailRenderer;
        _provider = mailProvider;
        _sender = sender;
    }

    #endregion

    #region Main Methods

    public async Task<Result> SendEmail(SendEmailDto dto)
    {
        var now = _clock.Now;

        var purpose = dto.Purpose.ToDomain();

        var spec = TemplateCatalog.Get(purpose);

        var missing = spec.RequiredKeys
            .Where(k => !dto.Data.TryGetValue(k, out var v) || string.IsNullOrWhiteSpace(v))
            .ToArray();

        if (missing.Length > 0)
        {
            _logger.LogWarning("Missing template keys for {Purpose}: {Keys}", purpose, string.Join(", ", missing));
            return Result.Failure(Error.Validation(
                ErrorCodes.Mail.TemplateMissingData,
                $"Missing required template data: {string.Join(", ", missing)}"));
        }

        var layout = await _provider.GetAsync(TemplateDefinitions.Layout.FileName);
        var content = await _provider.GetAsync(spec.FileName);

        var model = dto.Data.ToRenderModel();
        var html = _renderer.Render(layout, content, model);

        var log = new EmailLog(purpose, Email.FromPersisted(dto.To), now);
        await _mail.AddAsync(log);

        try
        {
            await _sender.SendHtmlAsync(dto.To, spec.Subject, html);

            log.MarkSent(now);
            await _mail.UpdateAsync(log);

            _logger.LogInformation("Email sent. Purpose={Purpose}, To={To}, LogId={LogId}", purpose, dto.To, log.Id);
            return Result.Success(successCode: "mail.sent");
        }
        catch (Exception ex)
        {
            // nếu bạn có MarkFailed/LastError thì set ở đây
            log.MarkFailed();
            await _mail.UpdateAsync(log);

            _logger.LogError(ex, "Send email failed. Purpose={Purpose}, To={To}, LogId={LogId}", purpose, dto.To, log.Id);
            return Result.Failure(Error.Failure("Mail.SendFailed", "Failed to send email"));
        }
    }

    #endregion
}
