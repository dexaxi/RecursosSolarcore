using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ImportMarronero : MonoBehaviour
{
	const string Path = "Assets/ImporteMarronero/";
	public void ImportarMarroneramente()
	{
		Debug.Log("Importando Marroneramente");

		var consequencesLines = ConsequenceString();
		var biomesLines = BiomeString();
		var machinesLines = MachineString();

		var rawConsequences = RawConsequencesEnum(consequencesLines);

		// Create the file from raw consequences
		File.WriteAllText(Path + "EnviroConsequenceType.cs", rawConsequences);
	}

	[ContextMenu("Crear Enums")]
	void CreateEnums()
	{

		// Create the file from raw consequences
		File.WriteAllText(Path + "EnviroConsequenceType.cs", RawConsequencesEnum(ConsequenceString()));
		File.WriteAllText(Path + "EnviroBiomeType.cs", RawBiomesEnum(BiomeString()));
		File.WriteAllText(Path + "EnviroMachineType.cs", RawMachineEnum(MachineString()));
		File.WriteAllText(Path + "EnviroProblemType.cs", RawProblemsEnum(ProblemString()));
		File.WriteAllText(Path + "EnviroAlterationType.cs", RawAlterationsEnum(AlterationString()));
	}

	[ContextMenu("Crear ScriptableObjects")]
	void CreateScriptableObjects()
	{
		var consequences = CreateConsequences();
		var machines = CreateMachines();
		var problems = CreateProblems(consequences, machines);
		var alterations = CreateAlterations(problems);
		var biomes = CreateBiomes(alterations);
	}

	Dictionary<BiomeType, Biome> CreateBiomes(Dictionary<EnviroAlterationType, EnviroAlteration> alterations)
	{
		var biomeArray = Resources.LoadAll("ScriptableObjects/Biomes", typeof(Biome));
		var dict = new Dictionary<BiomeType, Biome>();
		foreach (Biome biome in biomeArray.Cast<Biome>())
		{
			biome.StartBiome();
			dict[biome.Type] = biome;
		}

		var biomeDTODict = ParseBiomeDict();

		foreach (var type in (BiomeType[])System.Enum.GetValues(typeof(BiomeType)))
		{
			if (!dict.ContainsKey(type))
			{
				var newBiome = ScriptableObject.CreateInstance<Biome>();

				string assetPath = Path + "/ScriptableObjects/" + type.ToString() + ".asset";
				AssetDatabase.CreateAsset(newBiome, assetPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = newBiome;

				newBiome.name = biomeDTODict[type].name;
				newBiome.Type = biomeDTODict[type].Type;
				newBiome.Description = biomeDTODict[type].Description;
				newBiome.Mesh = biomeDTODict[type].Mesh;
				newBiome.Material = biomeDTODict[type].Material;
                newBiome.grassBottomColor = biomeDTODict[type].grassBottomColor;
                newBiome.grassTopColor = biomeDTODict[type].grassTopColor;
                newBiome.tilePrefab = biomeDTODict[type].tilePrefab;
				newBiome.biomeWeight = biomeDTODict[type].biomeWeight;
				newBiome.Sprite = biomeDTODict[type].Sprite;
				newBiome.EnviroAlterations = biomeDTODict[type].EnviroAlterations;
			}
		}

		return dict;

	}

	Dictionary<BiomeType, Biome.BiomeDTO> ParseBiomeDict()
	{
		var dict = new Dictionary<BiomeType, Biome.BiomeDTO>();

		var lines = BiomeString().Split('\n');

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var tags = line.Split('|');

			var dto = new Biome.BiomeDTO();
			dto.name = tags[0].Trim();
			dto.Type = (BiomeType)System.Enum.Parse(typeof(BiomeType), tags[1].Trim());
			dto.Description = tags[2].Trim();
			dto.Mesh = FindMesh(tags[3].Trim());
			dto.Material = FindMaterial(tags[4].Trim());
			dto.tilePrefab = FindPrefab(tags[5].Trim());
			dto.biomeWeight = int.Parse(tags[6].Trim());
			dto.Sprite = FindSprite(tags[7].Trim());

			var alterations = tags[8].Trim().Split(',');
			foreach (var alteration in alterations)
			{
				dto.EnviroAlterations.Add((EnviroAlterationType)System.Enum.Parse(typeof(EnviroAlterationType), alteration));
			}

			dict[dto.Type] = dto;
		}

		return dict;

	}

	Dictionary<EnviroConsequenceType, EnviroConsequence.EnviroConsequenceDTO> ParseConsequenceDict()
	{
		var dict = new Dictionary<EnviroConsequenceType, EnviroConsequence.EnviroConsequenceDTO>();

		var lines = ConsequenceString().Split('\n');

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var tags = line.Split('|');

			var dto = new EnviroConsequence.EnviroConsequenceDTO();
			dto.name = tags[0].Trim();
			dto.Title = tags[1].Trim();
			dto.Description = tags[2].Trim();
			dto.Type = (EnviroConsequenceType)System.Enum.Parse(typeof(EnviroConsequenceType), tags[3].Trim());
			dto.Sprite = FindSprite(tags[4].Trim());
			dto.color = GetColor(tags[5].Trim());

			var problems = tags[6].Trim().Split(',');
			foreach (var problem in problems)
			{
				if (string.IsNullOrEmpty(problem) == false)
				{
					var problemType = (EnviroProblemType)System.Enum.Parse(typeof(EnviroProblemType), problem);
					dto.RelatedProblems.Add(problemType);
				}
			}

			dict[dto.Type] = dto;
		}

		return dict;
	}

	Dictionary<EnviroConsequenceType, EnviroConsequence> CreateConsequences()
	{
		var consequences = new Dictionary<EnviroConsequenceType, EnviroConsequence>();

		var consequenceArray = Resources.LoadAll("ScriptableObjects/Consequences", typeof(EnviroConsequence));
		foreach (var consequence in consequenceArray.Cast<EnviroConsequence>())
		{
			consequences[consequence.Type] = consequence;
		}

		var consequenceDTODict = ParseConsequenceDict();

		foreach (var type in (EnviroConsequenceType[])System.Enum.GetValues(typeof(EnviroConsequenceType)))
		{
			if (!consequences.ContainsKey(type))
			{
				var newConsequence = ScriptableObject.CreateInstance<EnviroConsequence>();

				string assetPath = Path + "/ScriptableObjects/" + type.ToString() + ".asset";
				AssetDatabase.CreateAsset(newConsequence, assetPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = newConsequence;

				newConsequence.name = consequenceDTODict[type].name;
				newConsequence.Title = consequenceDTODict[type].Title;
				newConsequence.Description = consequenceDTODict[type].Description;
				newConsequence.Type = consequenceDTODict[type].Type;
				newConsequence.Sprite = consequenceDTODict[type].Sprite;
				newConsequence.color = consequenceDTODict[type].color;
				//newConsequence.RelatedProblems = consequenceDTODict[type].RelatedProblems;

				consequences[type] = newConsequence;
			}
		}
		return consequences;
	}

	Dictionary<MachineType, Machine> CreateMachines()
	{
		var machines = new Dictionary<MachineType, Machine>();

		var machineArray = Resources.LoadAll("ScriptableObjects/Machines", typeof(Machine));
		foreach (var machine in machineArray.Cast<Machine>())
		{
			machines[machine.Type] = machine;
		}

		var machineDTODict = ParseMachineDict();

		foreach (var type in (MachineType[])System.Enum.GetValues(typeof(MachineType)))
		{
			if (!machines.ContainsKey(type))
			{
				var newMachine = ScriptableObject.CreateInstance<Machine>();

				string assetPath = Path + "/ScriptableObjects/" + type.ToString() + ".asset";
				AssetDatabase.CreateAsset(newMachine, assetPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = newMachine;

				newMachine.name = machineDTODict[type].name;
				newMachine.Type = machineDTODict[type].Type;
				newMachine.Description = machineDTODict[type].Description;
				newMachine.Cost = machineDTODict[type].Cost;
				newMachine.MeshFilter = machineDTODict[type].MeshFilter;
				newMachine.MeshRenderer = machineDTODict[type].MeshRenderer;
				newMachine.ShopSprite = machineDTODict[type].ShopSprite;
				newMachine.RangePattern = machineDTODict[type].RangePattern;
				newMachine.CompatibleBiomes = machineDTODict[type].CompatibleBiomes;

				machines[type] = newMachine;
			}
		}
		return machines;
	}

	Dictionary<MachineType, Machine.MachineDTO> ParseMachineDict()
	{
		var dict = new Dictionary<MachineType, Machine.MachineDTO>();

		var lines = MachineString().Split('\n');

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var tags = line.Split('|');

			var dto = new Machine.MachineDTO();
			dto.name = tags[0].Trim();
			dto.Type = (MachineType)System.Enum.Parse(typeof(MachineType), tags[1].Trim());
			dto.Cost = float.Parse(tags[2].Trim());
			dto.MeshFilter = FindMesh(tags[3].Trim());
			dto.MeshRenderer = FindMaterial(tags[4].Trim());
			dto.ShopSprite = FindSprite(tags[5].Trim());
			dto.RangePattern = FindTexture(tags[6].Trim());
			dto.OptimizationLevel = int.Parse(tags[7].Trim());
			dto.Description = tags[9].Trim();

			var biomes = tags[8].Trim().Split(',');
			foreach (var biome in biomes)
			{
				dto.CompatibleBiomes.Add((BiomeType)System.Enum.Parse(typeof(BiomeType), biome));
			}

			dict[dto.Type] = dto;
		}

		return dict;
	}

	Dictionary<EnviroProblemType, EnviroProblem> CreateProblems(Dictionary<EnviroConsequenceType, EnviroConsequence> consequences, Dictionary<MachineType, Machine> machines)
	{
		var problems = new Dictionary<EnviroProblemType, EnviroProblem>();

		var problemArray = Resources.LoadAll("ScriptableObjects/Problems", typeof(EnviroProblem));
		foreach (var problem in problemArray.Cast<EnviroProblem>())
		{
			problems[problem.Type] = problem;
		}

		var problemDTODict = ParseProblemDict();

		foreach (var type in (EnviroProblemType[])System.Enum.GetValues(typeof(EnviroProblemType)))
		{
			if (!problems.ContainsKey(type))
			{
				var newProblem = ScriptableObject.CreateInstance<EnviroProblem>();

				string assetPath = Path + "/ScriptableObjects/" + type.ToString() + ".asset";
				AssetDatabase.CreateAsset(newProblem, assetPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = newProblem;

				newProblem.name = problemDTODict[type].name;
				newProblem.Title = problemDTODict[type].Title;
				newProblem.Description = problemDTODict[type].Description;
				newProblem.Type = problemDTODict[type].Type;
				newProblem.Section = problemDTODict[type].Section;
				newProblem.Icon = problemDTODict[type].Icon;
				newProblem.color = problemDTODict[type].color;


				newProblem.RelatedConsecuences = problemDTODict[type].RelatedConsecuences;
				newProblem.RelatedProblems = problemDTODict[type].RelatedProblems;
				newProblem.PossibleSolutions = problemDTODict[type].PossibleSolutions;

				problems[type] = newProblem;
			}
		}


		return problems;
	}

	Dictionary<EnviroProblemType, EnviroProblem.EnviroProblemDTO> ParseProblemDict()
	{
		var dict = new Dictionary<EnviroProblemType, EnviroProblem.EnviroProblemDTO>();

		var lines = ProblemString().Split('\n');

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var tags = line.Split('|');

			var dto = new EnviroProblem.EnviroProblemDTO();
			dto.name = tags[0].Trim();
			dto.Title = tags[1].Trim();
			dto.Description = tags[2].Trim();
			dto.Type = (EnviroProblemType)System.Enum.Parse(typeof(EnviroProblemType), tags[3].Trim());
			dto.Section = (EnviroProblemSection)System.Enum.Parse(typeof(EnviroProblemSection), tags[4].Trim());
			dto.Icon = FindSprite(tags[5].Trim());
			dto.color = GetColor(tags[6].Trim());
			dto.PossibleSolutions = tags[7].Trim().Split(',').Where(x => string.IsNullOrEmpty(x) == false).Select(x => (MachineType)System.Enum.Parse(typeof(MachineType), x)).ToList();

			dto.RelatedConsecuences = tags[8].Trim().Split(',').Where(x => string.IsNullOrEmpty(x) == false).Select(x => (EnviroConsequenceType)System.Enum.Parse(typeof(EnviroConsequenceType), x)).ToList();
			dto.RelatedProblems = tags[9].Trim().Split(',').Where(x => string.IsNullOrEmpty(x) == false).Select(x => (EnviroProblemType)System.Enum.Parse(typeof(EnviroProblemType), x)).ToList();

			dict[dto.Type] = dto;
		}

		return dict;
	}

	Dictionary<EnviroAlterationType, EnviroAlteration> CreateAlterations(Dictionary<EnviroProblemType, EnviroProblem> problems)
	{
		var alterations = new Dictionary<EnviroAlterationType, EnviroAlteration>();

		var alterationArray = Resources.LoadAll("ScriptableObjects/Alterations", typeof(EnviroAlteration));
		foreach (var alteration in alterationArray.Cast<EnviroAlteration>())
		{
			alterations[alteration.Type] = alteration;
		}

		var alterationDTODict = ParseAlterationDict();

		foreach (var type in (EnviroAlterationType[])System.Enum.GetValues(typeof(EnviroAlterationType)))
		{
			if (!alterations.ContainsKey(type))
			{
				var newAlteration = ScriptableObject.CreateInstance<EnviroAlteration>();

				string assetPath = Path + "/ScriptableObjects/" + type.ToString() + ".asset";
				AssetDatabase.CreateAsset(newAlteration, assetPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = newAlteration;

				newAlteration.name = alterationDTODict[type].name;
				newAlteration.Title = alterationDTODict[type].Title;
				newAlteration.DescriptionList = alterationDTODict[type].DescriptionList;
				newAlteration.Type = alterationDTODict[type].Type;
				newAlteration.SpriteDescriptions = alterationDTODict[type].SpriteDescriptions;
				newAlteration.Icon = alterationDTODict[type].Icon;
				newAlteration.color = alterationDTODict[type].color;
				newAlteration.Biome = alterationDTODict[type].Biome;
				newAlteration.EnviroProblems = alterationDTODict[type].EnviroProblems;

				alterations[type] = newAlteration;
			}
		}

		return alterations;
	}

	Dictionary<EnviroAlterationType, EnviroAlteration.EnviroAlterationDTO> ParseAlterationDict()
	{
		var dict = new Dictionary<EnviroAlterationType, EnviroAlteration.EnviroAlterationDTO>();

		var lines = AlterationString().Split('\n');

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var tags = line.Split('|');

			var dto = new EnviroAlteration.EnviroAlterationDTO();
			dto.name = tags[0].Trim();
			dto.Title = tags[1].Trim();
			dto.DescriptionList = new List<string>(tags[2].Trim().Split(';').Where(x => string.IsNullOrEmpty(x) == false));
			dto.Type = (EnviroAlterationType)System.Enum.Parse(typeof(EnviroAlterationType), tags[3].Trim());
			dto.SpriteDescriptions = tags[4].Trim().Split(',').Where(x => string.IsNullOrEmpty(x) == false).Select(x => FindSprite(x)).ToList();
			dto.Icon = FindSprite(tags[5].Trim());
			dto.color = GetColor(tags[6].Trim());
			dto.Biome = (BiomeType)System.Enum.Parse(typeof(BiomeType), tags[7].Trim());
			dto.EnviroProblems = tags[8].Trim().Split(',').Where(x => string.IsNullOrEmpty(x) == false).Select(x => (EnviroProblemType)System.Enum.Parse(typeof(EnviroProblemType), x)).ToList();

			//dto.Sprite = Resources.Load<Sprite>(tags[4].Trim());
			//dto.color = Resources.Load<Color>(tags[5].Trim());

			dict[dto.Type] = dto;
		}

		return dict;
	}


	string RawConsequencesEnum(string line)
	{
		var output = "public enum EnviroConsequenceType{";

		var lines = line.Split('\n');
		foreach (var consequence in lines)
		{
			if (string.IsNullOrEmpty(consequence))
			{
				continue;
			}

			var tags = consequence.Split('|');

			output += GetTypeTagName(tags[3].Trim()) + ", ";
		}

		output += "}";

		return output;
	}

	string RawBiomesEnum(string line)
	{
		var output = "public enum BiomeType{";

		var lines = line.Split('\n');
		foreach (var consequence in lines)
		{
			if (string.IsNullOrEmpty(consequence))
			{
				continue;
			}
			var tags = consequence.Split('|');

			output += GetTypeTagName(tags[1].Trim()) + ", ";
		}

		output += "}";

		return output;
	}

	string RawProblemsEnum(string line)
	{
		var output = "public enum EnviroProblemType{";

		var lines = line.Split('\n');
		foreach (var consequence in lines)
		{
			if (string.IsNullOrEmpty(consequence))
			{
				continue;
			}
			var tags = consequence.Split('|');

			output += GetTypeTagName(tags[3].Trim()) + ", ";
		}

		output += "}";

		return output;
	}

	string RawAlterationsEnum(string line)
	{
		var output = "public enum EnviroAlterationType{";

		var lines = line.Split('\n');
		foreach (var consequence in lines)
		{
			if (string.IsNullOrEmpty(consequence))
			{
				continue;
			}
			var tags = consequence.Split('|');

			output += GetTypeTagName(tags[3].Trim()) + ", ";
		}

		output += "}";

		return output;
	}

	string RawMachineEnum(string line)
	{
		var output = "public enum MachineType{";

		var lines = line.Split('\n');
		foreach (var consequence in lines)
		{
			if (string.IsNullOrEmpty(consequence))
			{
				continue;
			}
			var tags = consequence.Split('|');

			output += GetTypeTagName(tags[1].Trim()) + ", ";
		}

		output += "}";

		return output;
	}

	string ConsequenceString()
	{
		string consequences = "";
		consequences += "name | Title | Description | EnviroConsequenceType | Sprite | color | EnviroProblemType,EnviroProblemType1,\n";
		consequences += "name | Title | Description | EnviroConsequenceType2 | Sprite | color | EnviroProblemType,EnviroProblemType1,\n";

		return consequences;
	}

	string BiomeString()
	{
		string biomes = "";
		//biomes += "name | Type | Description | Mesh | Material | tilePrefab | biomeWeight | Sprite | EnviroAlterations \n";
		biomes += "name | BiomeType | Description | Mesh | Material | tilePrefab | 1 | Sprite | EnviroAlterationType, EnviroAlterationType2 \n";
		biomes += "name | BiomeType2 | Description | Mesh | Material | tilePrefab | 2 | Sprite | EnviroAlterationType, EnviroAlterationType2 \n";

		return biomes;
	}

	string MachineString()
	{
		string machines = "";
		//machines += "name  | TypeA | Cost | Mesh | Material | Sprite | RangePattern | OptimizationLevel | Description\n";
		machines += "name  | MachineTypeA | 1 | Mesh | Material | Sprite | RangePattern | 1 | BiomeType, BiomeType2| Description\n";
		machines += "name  | MachineTypeB | 2 | Mesh | Material | Sprite | RangePattern | 2 | BiomeType       | Description\n";
		machines += "name  | MachineTypeC | 3 | Mesh | Material | Sprite | RangePattern | 3 | BiomeType2      |  Description\n";
		machines += "name  | MachineTypeD | 4 | Mesh | Material | Sprite | RangePattern | 4 | BiomeType       |Description\n";


		return machines;
	}

	string ProblemString()
	{

		string problems = "";
		//problems += "name | Title | Description | EnviroProblemType | Wildlife| Sprite | color | TypeA, TypeD| EnviroConsequenceType,EnviroConsequenceType,|EnviroProblemType1\n";
		problems += "name | Title | Description | EnviroProblemType | Wildlife| Sprite | color | MachineTypeA, MachineTypeD| EnviroConsequenceType2,EnviroConsequenceType,|EnviroProblemType1\n";
		problems += "name | Title | Description | EnviroProblemType1 | Floor  | Sprite | color | MachineTypeB       |EnviroConsequenceType | EnviroProblemType, EnviroProblemType \n";

		return problems;
	}

	string AlterationString()
	{
		string alterations = "";
		alterations += "name | Title | Description; Description2; Description3 | EnviroAlterationType | Sprite,Sprite2 | Icon | color | BiomeType | EnviroProblemType1 \n";
		alterations += "name2 | Title2 | Description; Description2; Description3 | EnviroAlterationType2 | Sprite,Sprite2 | Icon | color | BiomeType2 | EnviroProblemType1 \n";
		

		return alterations;
	}

	string GetTypeTagName(string name)
	{
		name = name.Replace(" ", "_");

		//Turn string to pure ascii
		byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(name);
		name = System.Text.Encoding.ASCII.GetString(bytes);

		return name;
	}

	Sprite FindSprite(string text)
	{
		return null;
	}
	Color GetColor(string text)
	{
		return Color.white;
	}


	Mesh FindMesh(string text)
	{
		return null;
	}

	Material FindMaterial(string text)
	{
		return null;
	}

	GameObject FindPrefab(string text)
	{
		return null;
	}

	Texture2D FindTexture(string text)
	{

		return null;
	}
}