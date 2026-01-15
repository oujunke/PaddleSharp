using OpenCvSharp;

namespace Sdcb.PaddleOCR;

/// <summary>
///  A record struct representing a single character recognition result from an OCR operation.
/// </summary>
public record struct RecognizedChar
{
    /// <summary>
    /// A single character recognized from the image.
    /// </summary>
    public string Character { get; init; }

    /// <summary>
    /// The confidence score of the text recognition.
    /// </summary>
    public float Score { get; set; }

    /// <summary>
    /// The index position of this character within the recognized text.
    /// </summary>
    public int Index { get; init; }
    /// <summary>
    /// The current position of the text
    /// </summary>
    public Rect Rec { get; set; }
    /// <summary>
    ///  Initializes a new instance of the <see cref="RecognizedChar"/> record.
    /// </summary>
    /// <param name="character">The recognized character.</param>
    /// <param name="score">The confidence score of the character recognition.</param>
    /// <param name="index">The index position of this character within the recognized text.</param>
    /// <param name="rec">The current position of the text</param>
    public RecognizedChar(string character, float score, int index, Rect rec)
    {
        Character = character;
        Score = score;
        Index = index;
        Rec = rec;
    }
}