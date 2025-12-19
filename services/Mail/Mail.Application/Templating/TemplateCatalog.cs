using Mail.Domain.Enums;

namespace Mail.Application.Templating;

public static class TemplateCatalog
{
    public static TemplateSpec Get(EmailPurpose purpose) => TemplateDefinitions.For(purpose);
}