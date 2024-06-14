using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ImportMarronero : EditorWindow
{
	static string ImportMarroneroPath = "Assets/Resources/";
	static string DataPath = "Assets/_Data/";
	static string fullDataPath = "";
	 
	[ContextMenu("Crear Enums")]
    [MenuItem("EcoRescue/GenerateEnums")]
    static void CreateEnums()
	{
        fullDataPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), DataPath);
        // Create the file from raw consequences
        File.WriteAllText(ImportMarroneroPath + "EnviroConsequenceType.cs", RawConsequencesEnum(ConsequenceString()));
		File.WriteAllText(ImportMarroneroPath + "EnviroBiomeType.cs", RawBiomesEnum(BiomeString()));
		File.WriteAllText(ImportMarroneroPath + "EnviroMachineType.cs", RawMachineEnum(MachineString()));
		File.WriteAllText(ImportMarroneroPath + "EnviroProblemType.cs", RawProblemsEnum(ProblemString()));
		File.WriteAllText(ImportMarroneroPath + "EnviroAlterationType.cs", RawAlterationsEnum(AlterationString()));
	}

	[ContextMenu("Crear ScriptableObjects")]
    [MenuItem("EcoRescue/GenerateScriptableObjects")]
    static void CreateScriptableObjects()
	{
		fullDataPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), DataPath);
        var consequences = CreateConsequences();
		var machines = CreateMachines();
		var problems = CreateProblems(consequences, machines);
		var alterations = CreateAlterations(problems);
		var biomes = CreateBiomes(alterations);
		AssetDatabase.SaveAssets();
	}

	static Dictionary<BiomeType, Biome> CreateBiomes(Dictionary<EnviroAlterationType, EnviroAlteration> alterations)
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

				string assetPath = ImportMarroneroPath + "ScriptableObjects/" + "Biomes" + "/" + type.ToString() + ".asset";

				newBiome.name = biomeDTODict[type].name;
				newBiome.Type = biomeDTODict[type].Type;
				newBiome.Description = biomeDTODict[type].Description;
				newBiome.Mesh = biomeDTODict[type].Mesh;
				newBiome.Material = biomeDTODict[type].Material;
				newBiome.tilePrefab = biomeDTODict[type].tilePrefab;
				newBiome.biomeWeight = biomeDTODict[type].biomeWeight;
				newBiome.Sprite = biomeDTODict[type].Sprite;
				newBiome.EnviroAlterations = biomeDTODict[type].EnviroAlterations;
				newBiome.grassBottomColor = biomeDTODict[type].grassBottomColor;
				newBiome.grassTopColor = biomeDTODict[type].grassTopColor;

                AssetDatabase.CreateAsset(newBiome, assetPath);
                EditorUtility.SetDirty(newBiome);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = newBiome;

            }
        }

		return dict;

	}

	static Dictionary<BiomeType, Biome.BiomeDTO> ParseBiomeDict()
	{
		var dict = new Dictionary<BiomeType, Biome.BiomeDTO>();

		var lines = BiomeString();

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var tags = line.Split('|');

			var dto = new Biome.BiomeDTO();
			dto.name = tags[0].Trim();
			dto.Type = (BiomeType) Enum.Parse(typeof(BiomeType), tags[1].Trim());
			dto.Description = tags[2].Trim();
			dto.Mesh = FindMesh(tags[3].Trim());
			dto.Material = FindMaterial(tags[4].Trim());
			dto.grassBottomColor = GetColor(tags[5].Trim());
			dto.grassTopColor = GetColor(tags[6].Trim());
			dto.tilePrefab = FindMapPrefab(tags[7].Trim());
			dto.biomeWeight = int.Parse(tags[8].Trim());
			dto.Sprite = FindSprite(tags[9].Trim());

			var alterations = tags[10].Trim().Split(',');
			foreach (var alteration in alterations)
			{
				dto.EnviroAlterations.Add((EnviroAlterationType) Enum.Parse(typeof(EnviroAlterationType), alteration));
			}

			dict[dto.Type] = dto;
		}

		return dict;

	}

    static Dictionary<EnviroConsequenceType, EnviroConsequence.EnviroConsequenceDTO> ParseConsequenceDict()
	{
		var dict = new Dictionary<EnviroConsequenceType, EnviroConsequence.EnviroConsequenceDTO>();

		var lines = ConsequenceString();

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var tags = line.Split('|');

			var dto = new EnviroConsequence.EnviroConsequenceDTO();
			dto.name = tags[0].Trim();
			dto.Title = tags[1].Trim();
			dto.Description = tags[2].Trim();
			dto.Type = (EnviroConsequenceType) Enum.Parse(typeof(EnviroConsequenceType), tags[3].Trim());
			dto.Sprite = FindSprite(tags[4].Trim());
			dto.color = GetColor(tags[5].Trim());
			dict[dto.Type] = dto;
		}

		return dict;
	}

    static Dictionary<EnviroConsequenceType, EnviroConsequence> CreateConsequences()
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

				string assetPath = ImportMarroneroPath + "ScriptableObjects/" + "Consequences" + "/" + type.ToString() + ".asset";

				newConsequence.name = consequenceDTODict[type].name;
				newConsequence.Title = consequenceDTODict[type].Title;
				newConsequence.Description = consequenceDTODict[type].Description;
				newConsequence.Type = consequenceDTODict[type].Type;
				newConsequence.Sprite = consequenceDTODict[type].Sprite;
				newConsequence.color = consequenceDTODict[type].color;

                AssetDatabase.CreateAsset(newConsequence, assetPath);
                EditorUtility.SetDirty(newConsequence);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = newConsequence;

                consequences[type] = newConsequence;
			}
		}
		return consequences;
	}

    static Dictionary<MachineType, Machine> CreateMachines()
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

				string assetPath = ImportMarroneroPath + "ScriptableObjects/" + "Machines" + "/" + type.ToString() + ".asset";

				newMachine.name = machineDTODict[type].name;
				newMachine.Type = machineDTODict[type].Type;
				newMachine.Description = machineDTODict[type].Description;
				newMachine.title = machineDTODict[type].Title;
				newMachine.Cost = machineDTODict[type].Cost;
				newMachine.MeshFilter = machineDTODict[type].MeshFilter;
				newMachine.MeshRenderer = machineDTODict[type].MeshRenderer;
				newMachine.ShopSprite = machineDTODict[type].ShopSprite;
				newMachine.RangePattern = machineDTODict[type].RangePattern;
				newMachine.RestrictionType = machineDTODict[type].RestrictionType;
				newMachine.RestrictionTier = machineDTODict[type].RestrictionTier;
				newMachine.CompletionRateModifier = machineDTODict[type].CompletionRateModifier;

                AssetDatabase.CreateAsset(newMachine, assetPath);
                EditorUtility.SetDirty(newMachine);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = newMachine;
            }
        }
		return machines;
	}

    static Dictionary<MachineType, Machine.MachineDTO> ParseMachineDict()
	{
		var dict = new Dictionary<MachineType, Machine.MachineDTO>();

		var lines = MachineString();

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var tags = line.Split('|');

			var dto = new Machine.MachineDTO();
			dto.name = tags[0].Trim();
			dto.Type = (MachineType) Enum.Parse(typeof(MachineType), tags[1].Trim());
			dto.Title = tags[2].Trim();
			dto.Description = tags[3].Trim();
			dto.Cost = float.Parse(tags[4].Trim());
			dto.MeshFilter = FindMesh(tags[5].Trim());
			dto.MeshRenderer = FindMaterial(tags[6].Trim());
			dto.ShopSprite = FindSprite(tags[7].Trim());
			dto.RangePattern = FindPattern(tags[8].Trim());
			dto.RestrictionType = (MachineRestrictionType) Enum.Parse(typeof(MachineRestrictionType), tags[9].Trim());
			dto.RestrictionTier = int.Parse(tags[10].Trim());
			dto.CompletionRateModifier = int.Parse(tags[11].Trim());

			dict[dto.Type] = dto;
		}

		return dict;
	}

    static Dictionary<EnviroProblemType, EnviroProblem> CreateProblems(Dictionary<EnviroConsequenceType, EnviroConsequence> consequences, Dictionary<MachineType, Machine> machines)
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

				string assetPath = ImportMarroneroPath + "ScriptableObjects/" + "Problems" + "/" + type.ToString() + ".asset";

				newProblem.name = problemDTODict[type].name;
				newProblem.Title = problemDTODict[type].Title;
				newProblem.Description = problemDTODict[type].Description;
				newProblem.Type = problemDTODict[type].Type;
				newProblem.Section = problemDTODict[type].Section;
				newProblem.Phase = problemDTODict[type].Phase;
				newProblem.Icon = problemDTODict[type].Icon;
				newProblem.color = problemDTODict[type].color;


				newProblem.PossibleSolutions = problemDTODict[type].PossibleSolutions;
				newProblem.RelatedConsecuences = problemDTODict[type].RelatedConsecuences;
				newProblem.RelatedProblems = problemDTODict[type].RelatedProblems;

                AssetDatabase.CreateAsset(newProblem, assetPath);
                EditorUtility.SetDirty(newProblem);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = newProblem;


                problems[type] = newProblem;
			}
		}

		return problems;
	}

    static Dictionary<EnviroProblemType, EnviroProblem.EnviroProblemDTO> ParseProblemDict()
	{
		var dict = new Dictionary<EnviroProblemType, EnviroProblem.EnviroProblemDTO>();

		var lines = ProblemString();

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var tags = line.Split('|');

			var dto = new EnviroProblem.EnviroProblemDTO();
			dto.name = tags[0].Trim();
			dto.Title = tags[1].Trim();
			dto.Description = tags[2].Trim();
			dto.Type = (EnviroProblemType) Enum.Parse(typeof(EnviroProblemType), tags[3].Trim());
			dto.Section = (EnviroProblemSection) Enum.Parse(typeof(EnviroProblemSection), tags[4].Trim());
			dto.Phase = int.Parse(tags[5].Trim());
			dto.Icon = FindSprite(tags[6].Trim());
			dto.color = GetColor(tags[7].Trim());

			dto.PossibleSolutions = tags[8].Trim().Split(',')
				.Where(x => string.IsNullOrEmpty(x) == false)
				.Select(x => (MachineType) Enum.Parse(typeof(MachineType), x)).ToList();

			dto.RelatedConsecuences = tags[9].Trim().Split(',')
				.Where(x => string.IsNullOrEmpty(x) == false)
				.Select(x => (EnviroConsequenceType) Enum.Parse(typeof(EnviroConsequenceType), x)).ToList();
			
			dto.RelatedProblems = tags[10].Trim().Split(',')
				.Where(x => string.IsNullOrEmpty(x) == false)
				.Select(x => (EnviroProblemType) Enum.Parse(typeof(EnviroProblemType), x)).ToList();

			dict[dto.Type] = dto;
		}

		return dict;
	}

    static Dictionary<EnviroAlterationType, EnviroAlteration> CreateAlterations(Dictionary<EnviroProblemType, EnviroProblem> problems)
	{
		var alterations = new Dictionary<EnviroAlterationType, EnviroAlteration>();

		var alterationArray = Resources.LoadAll("ScriptableObjects/Alterations", typeof(EnviroAlteration));
		foreach (var alteration in alterationArray.Cast<EnviroAlteration>())
		{
			alterations[alteration.Type] = alteration;
		}

		var alterationDTODict = ParseAlterationDict();

		foreach (var type in (EnviroAlterationType[]) Enum.GetValues(typeof(EnviroAlterationType)))
		{
			if (!alterations.ContainsKey(type))
			{
				var newAlteration = ScriptableObject.CreateInstance<EnviroAlteration>();

				string assetPath = ImportMarroneroPath + "ScriptableObjects/" + "Alterations" + "/" + type.ToString() + ".asset";
				newAlteration.name = alterationDTODict[type].name;
				newAlteration.Title = alterationDTODict[type].Title;
				newAlteration.DescriptionList = alterationDTODict[type].DescriptionList;
				newAlteration.Type = alterationDTODict[type].Type;
				newAlteration.SpriteDescriptions = alterationDTODict[type].SpriteDescriptions;
				newAlteration.Icon = alterationDTODict[type].Icon;
				newAlteration.color = alterationDTODict[type].color;
				newAlteration.EnviroProblems = alterationDTODict[type].EnviroProblems;

                AssetDatabase.CreateAsset(newAlteration, assetPath);
                EditorUtility.SetDirty(newAlteration);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = newAlteration;


                alterations[type] = newAlteration;
			}
		}

		return alterations;
	}

    static Dictionary<EnviroAlterationType, EnviroAlteration.EnviroAlterationDTO> ParseAlterationDict()
	{
		var dict = new Dictionary<EnviroAlterationType, EnviroAlteration.EnviroAlterationDTO>();

		var lines = AlterationString();

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			var tags = line.Split('|');

			var dto = new EnviroAlteration.EnviroAlterationDTO();
			dto.name = tags[0].Trim();
			dto.Title = tags[1].Trim();
			
			dto.DescriptionList = new List<string>(tags[2].Trim().Split(';')
				.Where(x => string.IsNullOrEmpty(x) == false));
			
			dto.Type = (EnviroAlterationType) Enum.Parse(typeof(EnviroAlterationType), tags[3].Trim());
			
			dto.SpriteDescriptions = tags[4].Trim().Split(',')
				.Where(x => string.IsNullOrEmpty(x) == false).Select(x => FindSprite(x)).ToList();
			
			dto.Icon = FindSprite(tags[5].Trim());
			dto.color = GetColor(tags[6].Trim());

			dto.EnviroProblems = tags[7].Trim().Split(',')
				.Where(x => string.IsNullOrEmpty(x) == false)
				.Select(x => (EnviroProblemType) Enum.Parse(typeof(EnviroProblemType), x)).ToList();

			dict[dto.Type] = dto;
		}

		return dict;
	}


    static string RawConsequencesEnum(string[] lines)
	{
		var output = "public enum EnviroConsequenceType{";

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

	static string RawBiomesEnum(string[] lines)
	{
		var output = "public enum BiomeType{";

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

	static string RawProblemsEnum(string[] lines)
	{
		var output = "public enum EnviroProblemType{";

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

	static string RawAlterationsEnum(string[] lines)
	{
		var output = "public enum EnviroAlterationType{";

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

	static string RawMachineEnum(string[] lines)
	{
		var output = "public enum MachineType{";

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

    static string[] ConsequenceString()
	{
		var csvName = "EnviroConsequence.csv";
		
        var consequences = File.ReadAllLines(Path.Combine(fullDataPath, csvName), Encoding.UTF8);
		//consequences += "name | Title | Description | EnviroConsequenceType | Sprite | color | EnviroProblemType,EnviroProblemType1,\n";
		//consequences += "name | Title | Description | EnviroConsequenceType2 | Sprite | color | EnviroProblemType,EnviroProblemType1,\n";
		consequences = consequences[1..];

        return consequences;
	}

	static string[] BiomeString()
	{
        var csvName = "Biome.csv";

        var biomes = File.ReadAllLines(Path.Combine(fullDataPath, csvName), Encoding.UTF8);
        //biomes += "name | Type | Description | Mesh | Material | tilePrefab | biomeWeight | Sprite | EnviroAlterations \n";
        //biomes += "name | BiomeType | Description | Mesh | Material | tilePrefab | 1 | Sprite | EnviroAlterationType, EnviroAlterationType2 \n";
        //biomes += "name | BiomeType2 | Description | Mesh | Material | tilePrefab | 2 | Sprite | EnviroAlterationType, EnviroAlterationType2 \n";
        biomes = biomes[1..];

        return biomes;
	}

    static string[] MachineString()
	{
        var csvName = "Machine.csv";

        var machines = File.ReadAllLines(Path.Combine(fullDataPath, csvName), Encoding.UTF8);
        //machines += "name  | TypeA | Cost | Mesh | Material | Sprite | RangePattern | OptimizationLevel | Description\n";
        /*machines += "name  | MachineTypeA | 1 | Mesh | Material | Sprite | RangePattern | 1 | BiomeType, BiomeType2| Description\n";
		machines += "name  | MachineTypeB | 2 | Mesh | Material | Sprite | RangePattern | 2 | BiomeType       | Description\n";
		machines += "name  | MachineTypeC | 3 | Mesh | Material | Sprite | RangePattern | 3 | BiomeType2      |  Description\n";
		machines += "name  | MachineTypeD | 4 | Mesh | Material | Sprite | RangePattern | 4 | BiomeType       |Description\n";
		*/
        machines = machines[1..];

        return machines;
	}

    static string[] ProblemString()
	{
        var csvName = "EnviroProblem.csv";

        var problems = File.ReadAllLines(Path.Combine(fullDataPath, csvName), Encoding.UTF8);
        /*//problems += "name | Title | Description | EnviroProblemType | Wildlife| Sprite | color | TypeA, TypeD| EnviroConsequenceType,EnviroConsequenceType,|EnviroProblemType1\n";
		problems += "name | Title | Description | EnviroProblemType | Wildlife| Sprite | color | MachineTypeA, MachineTypeD| EnviroConsequenceType2,EnviroConsequenceType,|EnviroProblemType1\n";
		problems += "name | Title | Description | EnviroProblemType1 | Floor  | Sprite | color | MachineTypeB       |EnviroConsequenceType | EnviroProblemType, EnviroProblemType \n";
		*/
        problems = problems[1..];
        return problems;
	}

    static string[] AlterationString()
	{
        var csvName = "EnviroAlteration.csv";

        var alterations = File.ReadAllLines(Path.Combine(fullDataPath, csvName), Encoding.UTF8);
        /*alterations += "name | Title | Description; Description2; Description3 | EnviroAlterationType | Sprite,Sprite2 | Icon | color | BiomeType | EnviroProblemType1 \n";
		alterations += "name2 | Title2 | Description; Description2; Description3 | EnviroAlterationType2 | Sprite,Sprite2 | Icon | color | BiomeType2 | EnviroProblemType1 \n";
		*/
        alterations = alterations[1..];

        return alterations;
	}

    static string GetTypeTagName(string name)
	{
		name = name.Replace(" ", "_");

		//Turn string to pure ascii
		byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(name);
		name = System.Text.Encoding.ASCII.GetString(bytes);

		return name;
	}

    static string SpritePath = "Assets/2DAssets/Sprites/";
    static Sprite FindSprite(string text)
	{
        return AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath + text + ".png");
    }

    static Color GetColor(string text)
	{
        if (ColorUtility.TryParseHtmlString(text, out Color returnCol)) return returnCol;
        return Color.white;
	}

    static string ModelPath = "Assets/3DAssets/Models/";
    static Mesh FindMesh(string text)
	{
        return AssetDatabase.LoadAssetAtPath<Mesh>(ModelPath + text + ".fbx");
    }

    static string MaterialPath = "Assets/Materials/";
    static Material FindMaterial(string text)
	{
        return AssetDatabase.LoadAssetAtPath<Material>(MaterialPath + text + ".mat");
    }

    static string PrefabPath = "Assets/Prefabs/Map/";
    static GameObject FindMapPrefab(string text)
	{
		return AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath + text + "Cell.prefab"); ;
	}

    static string PatternPath = "Assets/2DAssets/MachinePatterns/";
    static Texture2D FindPattern(string text)
	{
		var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(PatternPath + text + ".png");
        return asset;
	}
}

#endif