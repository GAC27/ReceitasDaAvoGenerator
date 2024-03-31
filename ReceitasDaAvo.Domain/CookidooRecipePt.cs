using Newtonsoft.Json;

namespace ReceitasDaAvo.Domain
{
	public class CookidooRecipePt
	{
        [JsonProperty("Dificuldade")]
        public string Difficulty { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

        [JsonProperty("ingredients")]
		public IEnumerable<string> Ingredients { get; set; }

		[JsonProperty("language")]
		public string Language { get; set; }

		[JsonProperty("nutritions")]
		public IDictionary<string, string> Nutritions { get; set; }

		[JsonProperty("Porções")]
		public string Portions { get; set; }

		[JsonProperty("rating_count")]
		public int RatingCount { get; set; }

		[JsonProperty("rating_score")]
		public float RatingScore { get; set; }

		[JsonProperty("steps")]
		public IEnumerable<string> Steps { get; set; }

		[JsonProperty("tags")]
		public IEnumerable<string> Tags { get; set; }

		[JsonProperty("Tempo de preparação")]
		public string PreparationTime { get; set; }

		[JsonProperty("Tempo total")]
		public string TotalTime { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("tm-versions")]
		public IEnumerable<string> ThermomixVersions { get; set; }
	}
}
