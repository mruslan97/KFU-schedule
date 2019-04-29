using Microsoft.Extensions.Primitives;

namespace Schedule.Models
{
    /// <summary> Параметры коммандной строки </summary>
    public class QueryParameter
    {
        /// <summary> констуктор </summary>
        public QueryParameter(StringValues @params)
        {
            Params = @params;
        }

        /// <summary> параметры, которые отправил клиент </summary>
        public StringValues Params { get; set; }

        /// <summary> был ли обработан данный запрос </summary>
        public bool WasHandled { get; set; }
    }
}