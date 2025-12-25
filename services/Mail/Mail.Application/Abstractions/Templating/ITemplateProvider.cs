namespace Mail.Application.Abstractions.Templating;

public interface ITemplateProvider
{
    Task<string> GetAsync(string fileName);
}
public interface ITemplateRenderer
{
    string Render(string layoutHtml, string contentHtml, object model);
}