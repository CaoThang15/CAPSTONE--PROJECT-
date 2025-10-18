namespace SMarket.Business.Mappers
{
    public interface ICustomMapper
    {
        TDestination Map<TDestination>(object source) where TDestination : new();
        TDestination Map<TSource, TDestination>(TSource source) where TDestination : new();
        void Map<TSource, TDestination>(TSource source, TDestination destination);

        IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> sources) where TDestination : new();
        List<TDestination> MapToList<TSource, TDestination>(IEnumerable<TSource> sources) where TDestination : new();
    }
}
