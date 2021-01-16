namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Common.Searching
{
    public interface ISearchable
    {
#if SAMPLE_LOAD_CODE
        bool MatchesKeyword(string keyword);
#endif
    }
}
