using Bondy.Contracts.Dtos.Mail;
using Bondy.SharedKernel.Abstractions;
using Bondy.SharedKernel.Application;
using Bondy.SharedKernel.Common;
using Bondy.SharedKernel.Configuration;
using Bondy.SharedKernel.Constants;
using Mail.Application.Abstractions.Repositories;
using Mail.Application.Abstractions.Templating;
using Mail.Application.Mapper;
using Mail.Application.Templating;
using Mail.Domain.Constants;
using Mail.Domain.Entities;
using Mail.Domain.Enums;
using Mail.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Mail.Application.Services.Mail;

public sealed class MailService : ApplicationServiceBase, IMailService
{
    #region Constructor

    private readonly IMailRepository _mail;
    private readonly ITemplateRenderer _renderer;
    private readonly ITemplateProvider _provider;
    private readonly IEmailSender _sender;


    #endregion

    #region Main Methods

    public MailService(ILogger<MailService> logger, IClock clock, IOptions<AppConfigOptions> options, IMailRepository mail, ITemplateRenderer renderer, ITemplateProvider provider, IEmailSender sender) : base(logger, clock, options.Value)
    {
        _mail = mail;
        _renderer = renderer;
        _provider = provider;
        _sender = sender;
    }

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

        var dedupKey = BuildDedupKey(purpose, dto);

        var outbox = new EmailOutbox(
            purpose,
            Email.FromPersisted(dto.To),
            spec.Subject,
            JsonSerializer.Serialize(model),
            html,
            dedupKey,
            now);

        try
        {
            await _mail.AddAsync(outbox);
        }
        catch (Exception e)
        {
            _logger.LogError("Email enqueued fail Purpose={Purpose}, To={To}, DedupTokenId={DedupTokenId}", purpose, dto.To, dto.DedupTokenId ?? "");
        }

        _logger.LogInformation("Email enqueued. Purpose={Purpose}, To={To}, OutboxId={LogId}", purpose, dto.To, outbox.Id);

        return Result.Success(SuccessCodes.Mail.Enqueued);
    }


    #endregion

    #region Support Methods

    private static string BuildDedupKey(
        EmailPurpose purpose,
        SendEmailDto dto)
    {
        return purpose switch
        {
            EmailPurpose.Registration =>
                MailDedupKey.Otp(dto.To, dto.Purpose.ToString(), dto.DedupTokenId!),

            EmailPurpose.ResetPassword =>
                MailDedupKey.Otp(dto.To, dto.Purpose.ToString(), dto.DedupTokenId!),

            EmailPurpose.Welcome =>
                MailDedupKey.Welcome(dto.DedupTokenId!),
            
            EmailPurpose.OAuth2Welcome =>
                MailDedupKey.OAuth2Welcome(dto.DedupTokenId!),

            _ => throw new ArgumentOutOfRangeException(nameof(purpose), purpose, null)
        };
    }


    #endregion
}
