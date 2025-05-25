namespace TabbyCat.Models.AiMediaResponses;

/// <summary>
/// 需要上传图片的ai模型
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TParameter"></typeparam>
public abstract class
    UploadSourceImageAiGenerateImageEditModelBase<TInput, TParameter> : AiMediaRequestModelBase<TInput, TParameter>
    where TInput : OnlyOneImageInput

{
    [JsonProperty("model")] public override string Model { get; set; } = "wanx2.1-imageedit";
}

/// <summary>
/// 只要上传一张图片的模型
/// </summary>
public class OnlyOneImageUploadAiGenerateImageEditModel : UploadSourceImageAiGenerateImageEditModelBase<
    OnlyOneImageUploadAiGenerateImageEditModel.OnlyOneImageAiGenerateImageInput, object>
{
    public class OnlyOneImageAiGenerateImageInput : GenerateFunctionImageInput
    {

        [JsonProperty("prompt")] public string Prompt { get; set; }

    }
}

/// <summary>
/// 涂鸦绘画模型
/// </summary>
public class GraffitiPaintingImageEditModel : UploadSourceImageAiGenerateImageEditModelBase<
    GraffitiPaintingImageEditModel.OnlyOneImageAiGenerateImageInput, object>
{
    public class OnlyOneImageAiGenerateImageInput : OnlyOneImageInput
    {
        [JsonProperty("prompt")] public string Prompt { get; set; }

        [JsonProperty("sketch_image_url")] public override string Image { get; set; }
    }
}

public class AvatarStylizationEditModel : UploadSourceImageAiGenerateImageEditModelBase<AvatarStylizationEditModel.AvatarStylizationInput,object>
{
    public class AvatarStylizationInput:OnlyOneImageInput
    {
        [JsonProperty("image_url")]
        public override string Image { get; set; }
        
        [JsonProperty("style_index")]
        public int StyleIndex { get; set; }
        
        [JsonProperty("style_ref_url")]
        public string StyleRefImage { get; set; }
        
        public bool ShouldSerializeStyleRefImage() => !string.IsNullOrEmpty(StyleRefImage);
    }
}

/// <summary>
/// 图像擦除完成
/// </summary>
public class ImageEraseCompletionEditModel : UploadSourceImageAiGenerateImageEditModelBase<
    ImageEraseCompletionEditModel.ImageEraseCompletionEditInput,
    ImageEraseCompletionEditModel.ImageEraseCompletionParameter>
{
    public ImageEraseCompletionEditModel()
    {
        Model = "image-erase-completion";
    }

    public class ImageEraseCompletionEditInput : OnlyOneImageInput
    {
        [JsonProperty("image_url")] public override string Image { get; set; }

        [JsonProperty("mask_url")] public string MaskImage { get; set; } = string.Empty;

        [JsonProperty("foreground_url")] public string ForegroundImage { get; set; } = string.Empty;
    }

    public class ImageEraseCompletionParameter
    {
        [JsonProperty("fast_mode")] public bool FastMode { get; set; }
    }
}

/// <summary>
/// 需要绘制mask的图片
/// </summary>
public class MaskImageUploadAiGenerateImageEditModel : UploadSourceImageAiGenerateImageEditModelBase<
    MaskImageUploadAiGenerateImageEditModel.MaskImageUploadImageInput, ImageCountParameter>
{
    public class MaskImageUploadImageInput : OnlyOneImageUploadAiGenerateImageEditModel.OnlyOneImageAiGenerateImageInput
    {
        [JsonProperty("mask_image_url")] public required string MaskImage { get; set; }
    }
}

public class OnlyOneImageInput
{
    [JsonProperty("base_image_url")] public virtual string Image { get; set; } = string.Empty;

}

public class GenerateFunctionImageInput : OnlyOneImageInput
{
    [JsonProperty("function")] public virtual string Function { get; set; }
}


public class ImageCountParameter
{
    [JsonProperty("n")] public int Count { get; set; } = 1;
}

public class StrengthParameter : ImageCountParameter
{
    [JsonProperty("strength")] public double Strength { get; set; } = 0.5;
}

public sealed class ExpandImageParameter : ImageCountParameter
{
    [JsonProperty("top_scale")] public double TopScale { get; set; } = 1;
    [JsonProperty("bottom_scale")] public double BottomScale { get; set; } = 1;
    [JsonProperty("left_scale")] public double LeftScale { get; set; } = 1;
    [JsonProperty("right_scale")] public double RightScale { get; set; } = 1;
}

public sealed class ImageSuperResolutionParameter : ImageCountParameter
{
    [JsonProperty("upscale_factor")] public int UpscaleFactor { get; set; } = 2;
}

public sealed class SketchParameter : ImageCountParameter
{
    [JsonProperty("is_sketch")] public bool IsSketch { get; set; }
}

public sealed class GraffitiPaintingParameter : ImageCountParameter
{
    [JsonProperty("size")] public string Size { get; set; }

    [JsonProperty("sketch_weight")] public int SketchWeight { get; set; }

    [JsonProperty("style")] public string Style { get; set; }
}