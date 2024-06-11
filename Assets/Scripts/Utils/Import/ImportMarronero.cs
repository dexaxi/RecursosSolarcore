using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

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
		var biomes = CreateBiomes();
		var machines = CreateMachines();
		var problems = CreateProblems();
		var alterations = CreateAlterations();
	}

	Dictionary<BiomeType, Biome> CreateBiomes()
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
			var tags = line.Split('|');

			var dto = new Biome.BiomeDTO();
			dto.name = tags[0].Trim();
			dto.Type = (BiomeType)System.Enum.Parse(typeof(BiomeType), tags[1].Trim());
			dto.Description = tags[2].Trim();
			//dto.Mesh = Resources.Load<Mesh>(tags[3].Trim());
			//dto.Material = Resources.Load<Material>(tags[4].Trim());
			//dto.tilePrefab = Resources.Load<GameObject>(tags[5].Trim());
			dto.biomeWeight = int.Parse(tags[6].Trim());
			//dto.Sprite = Resources.Load<Sprite>(tags[7].Trim());

			var alterations = tags[8].Trim().Split(',');
			foreach (var alteration in alterations)
			{
				dto.EnviroAlterations.Add((EnviroAlterationType)System.Enum.Parse(typeof(EnviroAlterationType), alteration));
			}
		}

		return dict;

	}

	Dictionary<EnviroConsequenceType, EnviroConsequence.EnviroConsequenceDTO> ParseConsequenceDict()
	{
		var dict = new Dictionary<EnviroConsequenceType, EnviroConsequence.EnviroConsequenceDTO>();

		var lines = ConsequenceString().Split('\n');

		foreach (var line in lines)
		{
			var tags = line.Split('|');

			var dto = new EnviroConsequence.EnviroConsequenceDTO();
			dto.name = tags[0].Trim();
			dto.Title = tags[1].Trim();
			dto.Description = tags[2].Trim();
			dto.Type = (EnviroConsequenceType)System.Enum.Parse(typeof(EnviroConsequenceType), tags[3].Trim());
			//dto.Sprite = Resources.Load<Sprite>(tags[4].Trim());
			//dto.color = Resources.Load<Color>(tags[5].Trim());

			var problems = tags[6].Trim().Split(',');
			foreach (var problem in problems)
			{
				dto.RelatedProblems.Add((EnviroProblemType)System.Enum.Parse(typeof(EnviroProblemType), problem));
			}
		}

		return dict;
	}

	Dictionary<EnviroConsequenceType, EnviroConsequence> CreateConsequences()
	{
		var consequences = new Dictionary<EnviroConsequenceType, EnviroConsequence>();

		var consequenceArray = Resources.LoadAll("ScriptableObjects/Machines", typeof(EnviroConsequence));
		var dict = new Dictionary<EnviroConsequenceType, EnviroConsequence>();
		foreach (var consequence in consequenceArray.Cast<EnviroConsequence>())
		{
			dict[consequence.Type] = consequence;
		}

		var consequenceDTODict = ParseConsequenceDict();

		foreach (var type in (EnviroConsequenceType[])System.Enum.GetValues(typeof(EnviroConsequenceType)))
		{
			if (!dict.ContainsKey(type))
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
			}
		}
		return consequences;


	}

	Dictionary<MachineType, Machine> CreateMachines()
	{
		var machines = new Dictionary<MachineType, Machine>();

		var machineArray = Resources.LoadAll("ScriptableObjects/Machines", typeof(Machine));
		var dict = new Dictionary<MachineType, Machine>();
		foreach (var machine in machineArray.Cast<Machine>())
		{
			dict[machine.Type] = machine;
		}

		var machineDTODict = ParseMachineDict();

		foreach (var type in (MachineType[])System.Enum.GetValues(typeof(MachineType)))
		{
			if (!dict.ContainsKey(type))
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
				//newMachine.EnviroAlterations = machineDTODict[type].EnviroAlterations;
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
			var tags = line.Split('|');

			var dto = new Machine.MachineDTO();
			dto.name = tags[0].Trim();
			dto.Type = (MachineType)System.Enum.Parse(typeof(MachineType), tags[1].Trim());
			dto.Description = tags[2].Trim();
			//dto.Sprite = Resources.Load<Sprite>(tags[3].Trim());
			//dto.color = Resources.Load<Color>(tags[4].Trim());

			throw new System.Exception("No se ha implementado la carga de alteraciones en las máquinas");
			/*
			var alterations = tags[5].Trim().Split(',');
			foreach (var alteration in alterations)
			{
				dto.EnviroAlterations.Add((EnviroAlterationType)System.Enum.Parse(typeof(EnviroAlterationType), alteration));
			}
			*/
		}

		return dict;
	}

	Dictionary<EnviroProblemType, EnviroProblem> CreateProblems()
	{
		var problems = new Dictionary<EnviroProblemType, EnviroProblem>();

		var problemArray = Resources.LoadAll("ScriptableObjects/Problems", typeof(EnviroProblem));
		var dict = new Dictionary<EnviroProblemType, EnviroProblem>();
		foreach (var problem in problemArray.Cast<EnviroProblem>())
		{
			dict[problem.Type] = problem;
		}

		var problemDTODict = ParseProblemDict();

		foreach (var type in (EnviroProblemType[])System.Enum.GetValues(typeof(EnviroProblemType)))
		{
			if (!dict.ContainsKey(type))
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
				//newProblem.Sprite = problemDTODict[type].Sprite;
				//newProblem.color = problemDTODict[type].color;
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
			var tags = line.Split('|');

			var dto = new EnviroProblem.EnviroProblemDTO();
			dto.name = tags[0].Trim();
			dto.Title = tags[1].Trim();
			dto.Description = tags[2].Trim();
			dto.Type = (EnviroProblemType)System.Enum.Parse(typeof(EnviroProblemType), tags[3].Trim());
			//dto.Sprite = Resources.Load<Sprite>(tags[4].Trim());
			//dto.color = Resources.Load<Color>(tags[5].Trim());
		}

		return dict;
	}


	Dictionary<EnviroAlterationType, EnviroAlteration> CreateAlterations()
	{
		var alterations = new Dictionary<EnviroAlterationType, EnviroAlteration>();

		var alterationArray = Resources.LoadAll("ScriptableObjects/Alterations", typeof(EnviroAlteration));
		var dict = new Dictionary<EnviroAlterationType, EnviroAlteration>();
		foreach (var alteration in alterationArray.Cast<EnviroAlteration>())
		{
			dict[alteration.Type] = alteration;
		}

		var alterationDTODict = ParseAlterationDict();

		foreach (var type in (EnviroAlterationType[])System.Enum.GetValues(typeof(EnviroAlterationType)))
		{
			if (!dict.ContainsKey(type))
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
				//newAlteration.Description = alterationDTODict[type].Description;
				newAlteration.Type = alterationDTODict[type].Type;
				//newAlteration.Sprite = alterationDTODict[type].Sprite;
				//newAlteration.color = alterationDTODict[type].color;
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
			var tags = line.Split('|');

			var dto = new EnviroAlteration.EnviroAlterationDTO();
			dto.name = tags[0].Trim();
			dto.Title = tags[1].Trim();
			//dto.Description = tags[2].Trim();
			dto.Type = (EnviroAlterationType)System.Enum.Parse(typeof(EnviroAlterationType), tags[3].Trim());
			//dto.Sprite = Resources.Load<Sprite>(tags[4].Trim());
			//dto.color = Resources.Load<Color>(tags[5].Trim());
		}

		return dict;
	}

	string RawConsequencesEnum(string line)
	{
		var output = "/*public enum EnviroConsequenceType{";

		var lines = line.Split('\n');
		foreach (var consequence in lines)
		{
			if (string.IsNullOrEmpty(consequence))
			{
				continue;
			}

			var tags = consequence.Split('|');

			output += GetTypeTagName(tags[0].Trim()) + ", ";
		}

		output += "}*/";

		return output;
	}

	string RawBiomesEnum(string line)
	{
		var output = "/*public enum BiomeType{";

		var lines = line.Split('\n');
		foreach (var consequence in lines)
		{
			if (string.IsNullOrEmpty(consequence))
			{
				continue;
			}
			var tags = consequence.Split('|');

			output += GetTypeTagName(tags[0].Trim()) + ", ";
		}

		output += "}*/";

		return output;
	}

	string RawProblemsEnum(string line)
	{
		var output = "/*public enum EnviroProblemType{";

		var lines = line.Split('\n');
		foreach (var consequence in lines)
		{
			if (string.IsNullOrEmpty(consequence))
			{
				continue;
			}
			var tags = consequence.Split('|');

			output += GetTypeTagName(tags[0].Trim()) + ", ";
		}

		output += "}*/";

		return output;
	}

	string RawAlterationsEnum(string line)
	{
		var output = "/*public enum EnviroAlterationType{";

		var lines = line.Split('\n');
		foreach (var consequence in lines)
		{
			if (string.IsNullOrEmpty(consequence))
			{
				continue;
			}
			var tags = consequence.Split('|');

			output += GetTypeTagName(tags[0].Trim()) + ", ";
		}

		output += "}*/";

		return output;
	}

	string RawMachineEnum(string line)
	{
		var output = "/*public enum MachineType{";

		var lines = line.Split('\n');
		foreach (var consequence in lines)
		{
			if (string.IsNullOrEmpty(consequence))
			{
				continue;
			}
			var tags = consequence.Split('|');

			output += GetTypeTagName(tags[0].Trim()) + ", ";
		}

		output += "}*/";

		return output;
	}

	string ConsequenceString()
	{
		string consequences = "";
		consequences += "name | Title | Description | EnviroConsequenceType | Sprite | color \n";

		return consequences;
	}

	string BiomeString()
	{
		string biomes = "";
		biomes += "name | Type | Description | Mesh | Material | tilePrefab | biomeWeight | Sprite | EnviroAlterations \n";

		return biomes;
	}

	string MachineString()
	{
		string machines = "";
		machines += "name  | Type | Description | Sprite | color | EnviroAlterations \n";
		machines += "name2 | Type | Description | Sprite | color | EnviroAlterations \n";
		machines += "name3 | Type | Description | Sprite | color | EnviroAlterations \n";
		machines += "name4 | Type | Description | Sprite | color | EnviroAlterations \n";

		return machines;
	}

	string ProblemString()
	{
		string problems = "";
		problems += "name | Title | Description | EnviroProblemType | Sprite | color \n";

		return problems;
	}

	string AlterationString()
	{
		string alterations = "";
		alterations += "name | Title | Description | EnviroAlterationType | Sprite | color \n";

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
}