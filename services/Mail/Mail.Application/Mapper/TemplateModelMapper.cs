namespace Mail.Application.Mapper;

public static class TemplateModelMapper
{
    public static object ToRenderModel(this Dictionary<string, string> data)
        => data; // Scriban hỗ trợ dictionary access dạng {{ firstName }} nếu member renamer đúng.
}
