using Newtonsoft.Json;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using ReceitasDaAvo.Domain;
using System.Text;

//Set the license of QuestPDF because we need it to use it in basically the open source mode
Settings.License = LicenseType.Community;
Settings.CheckIfAllTextGlyphsAreAvailable = false;

IEnumerable<CookidooRecipePt>? cookidooRecipes;

Console.WriteLine("Insert the path to the file with the recipes (e.g: c:\\Recipes\\recipeDump.json)");
var recipesFilePath = Console.ReadLine();
ArgumentException.ThrowIfNullOrWhiteSpace(recipesFilePath);

Console.WriteLine("Insert the path to the folder with the images (e.g: c:\\Recipes\\recipeDump\\Images). " +
	"The images should have the format <recipeId>.jpg.");
var imagesPath = Console.ReadLine();
ArgumentException.ThrowIfNullOrWhiteSpace(imagesPath);

Console.WriteLine("Insert the path to save the recipe pdf file (e.g: c:\\RecipesFormated). If empty it will store in the root of where this is executing.");
var savePath = Console.ReadLine();

Console.WriteLine("With what name do you want to save the file? (e.g: BestDesertsEver)");
var saveFileName = Console.ReadLine();
ArgumentException.ThrowIfNullOrWhiteSpace(saveFileName);


using (StreamReader r = new StreamReader(recipesFilePath))
{
	string jsonRecipes = r.ReadToEnd();

	cookidooRecipes = JsonConvert.DeserializeObject<IEnumerable<CookidooRecipePt>>(jsonRecipes);
	ArgumentNullException.ThrowIfNull(cookidooRecipes);
}

var document = Document.Create(container =>
{
	Console.Clear();
	var processedRecipesCounter = 1;
	var totalRecipes = cookidooRecipes.Count();
	foreach (var recipe in cookidooRecipes)
	{
		Console.WriteLine($"Processing recipe #{processedRecipesCounter++} out of {totalRecipes} ...");

		container.Page(page =>
		{
			page.Size(PageSizes.A4); //595.4f points of size
			page.Margin(2, Unit.Centimetre);
			page.PageColor(Colors.White);
			page.DefaultTextStyle(x => x.FontSize(12));

			page.Content()
				.PaddingVertical(1, Unit.Centimetre)
				.Column(col =>
				{
					col.Item().Row(row =>
					{
						row.RelativeItem().Text(recipe.Title).SemiBold().FontSize(28).FontColor(Colors.Green.Darken1);
					});

					col.Item().Row(row =>
					{
						if (File.Exists($"{imagesPath}\\{recipe.Id}.jpg"))
						{
							row.RelativeItem().Image($"{imagesPath}\\{recipe.Id}.jpg");
						}
					});

					col.Item().PageBreak();


					col.Item().Row(row =>
					{
						row.RelativeItem(0.4f) //Left Column
						.BorderTop(1)
						.BorderColor(Colors.Green.Darken1)
						.Column(ingredientCol =>
						{
							ingredientCol.Item().PaddingBottom(10).Text("Ingredients: ").SemiBold().FontSize(12);

							foreach (var ingredient in recipe.Ingredients)
							{
								ingredientCol.Item().PaddingBottom(10).Text(ingredient + ";").FontSize(10);
							}


							ingredientCol.Item().Column(n =>
							{
								n.Item().Text("Nutrional information: ").SemiBold().FontSize(12);

								var tagsAll = new StringBuilder();
								foreach (var nutrition in recipe.Nutritions)
								{
									n.Item().Text($"{nutrition.Key}: {nutrition.Value}").FontSize(10);
								}
							});
						});

						row.RelativeItem(0.1f); // empty column just to create space

						row.RelativeItem()
							.Column(recipeCol =>
							{
								recipeCol.Item().PaddingBottom(0.5f, Unit.Centimetre).Row(detailsRow =>
								{
									detailsRow.RelativeItem().Column(dificultyColumn =>
									{
										dificultyColumn.Item().Text("Difficulty: ").SemiBold().FontSize(14);
										dificultyColumn.Item().Text(recipe.Difficulty);
									});
									detailsRow.RelativeItem().Column(prepTimeColumn =>
									{
										prepTimeColumn.Item().Text("Preparation Time: ").SemiBold().FontSize(14);
										prepTimeColumn.Item().Text(recipe.PreparationTime);
									});
									detailsRow.RelativeItem().Column(totalTimeColumn =>
									{
										totalTimeColumn.Item().Text("Total Time: ").SemiBold().FontSize(14);
										totalTimeColumn.Item().Text(recipe.TotalTime);
									});
									detailsRow.RelativeItem().Column(portionsColumn =>
									{
										portionsColumn.Item().Text("Portions: ").SemiBold().FontSize(14);
										portionsColumn.Item().Text(recipe.Portions);
									});
								});

								recipeCol.Item().PaddingVertical(0.5f, Unit.Centimetre).Row(tagsAndTmModelsRow =>
								{
									tagsAndTmModelsRow.RelativeItem().Column(tmModelsColumn =>
									{
										tmModelsColumn.Item().Text("Thermomix models: ").SemiBold().FontSize(14);
										foreach (var tmVersion in recipe.ThermomixVersions)
										{
											tmModelsColumn.Item().Text(tmVersion);

										}
									});
									tagsAndTmModelsRow.RelativeItem().Column(tagsColumn =>
									{
										tagsColumn.Item().Text("Tags: ").SemiBold().FontSize(14);

										var tagsAll = new StringBuilder();
										foreach (var tag in recipe.Tags)
										{
											tagsAll.Append(tag);
											tagsAll.Append("; ");
										}
										tagsColumn.Item().Text(tagsAll.ToString());

									});
								});

								recipeCol.Item().PaddingVertical(0.5f, Unit.Centimetre).Row(stepsCol =>
								{
									stepsCol.RelativeItem().Column(steps =>
									{
										steps.Item().Text("Steps: ").SemiBold().FontSize(16);

										var stepCounter = 1;
										foreach (var step in recipe.Steps)
										{
											steps.Item()
												.PaddingVertical(0.1f, Unit.Centimetre)
												.Text($"{stepCounter}. {step}");
											stepCounter++;
										}

									});
								});
							});
					});
					col.Item().PageBreak();
				});

			page.Footer()
				.AlignLeft()
				.Column(col =>
				{
					col.Item().Row(row =>
					{
						row.RelativeItem().Text(recipe.Id);
						row.RelativeItem().AlignCenter().Text(recipe.Title);
						row.RelativeItem().AlignRight().Text(x =>
						{
							x.Span(" Page ");
							x.CurrentPageNumber();
						});
					});
				});
		});
		//break;	//For Debugging purposes and lazyness from my side
	}
});

//Create Path if it doesn't exist
Directory.CreateDirectory(savePath);

// instead of the standard way of generating a PDF file
document.GeneratePdf(string.IsNullOrWhiteSpace(savePath)? $"{saveFileName}.pdf" : $"{savePath}\\{saveFileName}.pdf");

// use the following invocation
//document.ShowInPreviewer();

Console.ReadLine();
