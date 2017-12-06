using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FloatingBarsWindow : EditorWindow
{
	// Paths
	private const string spritePath = "Assets/FloatingBars/Sprites";

	// Mandatory Fields
	public GameObject targettedGameObject;
	public FloatVariable resourceVariable;
	public FloatVariable minResourceVariable;
	public FloatVariable maxResourceVariable;

	// Optional Fields
	public FloatVariable regenRate;
	public float posX = 0;
	public float posY = 2f;
	public float posZ = 0;
	public Color positiveColor = Color.green;
	public Color negativeColor = Color.red;
	public Sprite positiveImage;
	public Sprite negativeImage;

	string titleWindow = "ie : Health Points";
	bool groupEnabled;
	bool myBool = true;
	float myFloat = 1.23f;

	// Create FloatVariable Menu default values
	bool showCreateResource = false;
	bool showCreateMinResource = false;
	bool showCreateMaxResource = false;
	bool showCreateRegenRate = false;
	string floatVariableName = "new FloatVariable";
	float floatVariableValue = 0f;

	[MenuItem("Floating Bars/Open Creator", false, 1)]
	private static void ShowWindow()
	{
		FloatingBarsWindow window = (FloatingBarsWindow)GetWindow(typeof(FloatingBarsWindow));
		window.minSize = new Vector2(300, 300);
		window.maxSize = new Vector2(500, 400);
		window.titleContent = new GUIContent("Floating Bar");
		window.Show();
	}

	[MenuItem("Floating Bars/Create Default Bar (object have to be selected)", false, 2)] // adding priority in menu 1-50
	private static void CreateDefaultOption()
	{
		FloatingBarsWindow window = (FloatingBarsWindow)GetWindow(typeof(FloatingBarsWindow));
		window.Show();
	}
	
	// Validate that the object can receive a floating bar
	[MenuItem("Floating Bars/Create Default Bar (object have to be selected)", true)]
	private static bool CreateDefaultOptionValidation()
	{
		// Return true if we select a gameObject
		return Selection.activeObject.GetType() == typeof(GameObject);
	}

	private void OnGUI()
	{
		CreateWindowLayout();
		positiveImage = Resources.Load<Sprite>("FloatingBar");
		negativeImage = Resources.Load<Sprite>("FloatingBar");

		if (targettedGameObject == null
			|| resourceVariable == null || minResourceVariable == null || maxResourceVariable == null)
			GUI.enabled = false;

		if (GUILayout.Button("Add New Floating Bar")) {
			AddGeneratedBar();
		}
	}

	private void AddGeneratedBar()
	{
		// Load Floating Bar Prefab and add it to target
		GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/FloatingBars/Prefabs/FloatingBar Container.prefab", typeof(GameObject)) as GameObject;
		GameObject floatingBarPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

		// Modify elements with given values
		floatingBarPrefab.name = titleWindow;

		FloatingBarController floatingBarController = floatingBarPrefab.GetComponent<FloatingBarController>();
		floatingBarController.resource = resourceVariable;
		floatingBarController.Min = minResourceVariable;
		floatingBarController.Max = maxResourceVariable;

		Image backgroundImage = floatingBarPrefab.GetComponentsInChildren<Image>()[0];
		backgroundImage.color = negativeColor;
		backgroundImage.sprite = positiveImage;

		Image foregroundImage = floatingBarPrefab.GetComponentsInChildren<Image>()[1];
		foregroundImage.color = positiveColor;
		foregroundImage.sprite = negativeImage;

		floatingBarPrefab.transform.SetParent(targettedGameObject.transform);
		// Change local position after assigning to parent to avoid misplacement
		floatingBarPrefab.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY, posZ);

		// Add floating bar variable controller
		FloatingBarVariableController fbv_controller =  targettedGameObject.AddComponent<FloatingBarVariableController>();
		fbv_controller.currentValue = resourceVariable;
		fbv_controller.resetValue = true; // default to true ... to define
		fbv_controller.startingValue = maxResourceVariable;
		fbv_controller.RegenRate = regenRate;
	}

	private void CreateWindowLayout()
	{
		// Get Informations before create
		GUILayout.Label("Create New Bar", EditorStyles.boldLabel);
		titleWindow = EditorGUILayout.TextField("Floating Bar Name : ", titleWindow);

		GUILayout.Space(10f);

		GUILayout.Label("Mandatory Settings :", EditorStyles.label);

		targettedGameObject = EditorGUILayout.ObjectField(
			"Attach to : ",
			targettedGameObject,
			typeof(GameObject),
			true) as GameObject;

		resourceVariable = EditorGUILayout.ObjectField(
			"Resource Variable : ",
			resourceVariable,
			typeof(FloatVariable),
			true) as FloatVariable;

		showCreateResource = EditorGUILayout.Foldout(showCreateResource, "Add new FloatVariable");
		if (showCreateResource) {
			floatVariableName = EditorGUILayout.TextField("FloatVariable Name : ", floatVariableName);
			floatVariableValue = EditorGUILayout.FloatField("FloatVariable Value : ", floatVariableValue);

			if (GUILayout.Button("Create & Add FloatVariable")) {
				resourceVariable = CreateNewFloatVariable();
				showCreateResource = !showCreateResource;
			}
		}

		minResourceVariable = EditorGUILayout.ObjectField(
			"MinResource Variable : ",
			minResourceVariable,
			typeof(FloatVariable),
			true) as FloatVariable;

		showCreateMinResource = EditorGUILayout.Foldout(showCreateMinResource, "Add new FloatVariable");
		if (showCreateMinResource) {
			floatVariableName = EditorGUILayout.TextField("FloatVariable Name : ", floatVariableName);
			floatVariableValue = EditorGUILayout.FloatField("FloatVariable Value : ", floatVariableValue);

			if (GUILayout.Button("Create & Add FloatVariable")) {
				minResourceVariable = CreateNewFloatVariable();
				showCreateMinResource = !showCreateMinResource;
			}
		}

		maxResourceVariable = EditorGUILayout.ObjectField(
			"MaxHealthPoint Variable : ",
			maxResourceVariable,
			typeof(FloatVariable),
			true) as FloatVariable;

		showCreateMaxResource = EditorGUILayout.Foldout(showCreateMaxResource, "Add new FloatVariable");
		if (showCreateMaxResource) {
			floatVariableName = EditorGUILayout.TextField("FloatVariable Name : ", floatVariableName);
			floatVariableValue = EditorGUILayout.FloatField("FloatVariable Value : ", floatVariableValue);

			if (GUILayout.Button("Create & Add FloatVariable")) {
				maxResourceVariable = CreateNewFloatVariable();
				showCreateMaxResource = !showCreateMaxResource;
			}
		}

		regenRate = EditorGUILayout.ObjectField(
			"RegenRate Variable : ",
			regenRate,
			typeof(FloatVariable),
			true) as FloatVariable;

		showCreateRegenRate = EditorGUILayout.Foldout(showCreateRegenRate, "Add new FloatVariable");
		if (showCreateRegenRate) {
			floatVariableName = EditorGUILayout.TextField("FloatVariable Name : ", floatVariableName);
			floatVariableValue = EditorGUILayout.FloatField("FloatVariable Value : ", floatVariableValue);

			if (GUILayout.Button("Create & Add FloatVariable")) {
				regenRate = CreateNewFloatVariable();
				showCreateRegenRate = !showCreateRegenRate;
			}
		}

		GUILayout.Space(10f);

		GUILayout.Label("Optional Settings : (Default values will be applied)", EditorStyles.label);

		posX = EditorGUILayout.FloatField("Position X :", posX);
		posY = EditorGUILayout.FloatField("Position Y :", posY);
		posZ = EditorGUILayout.FloatField("Position Z :", posZ);
		positiveColor = EditorGUILayout.ColorField("Positive Color :", positiveColor);
		negativeColor = EditorGUILayout.ColorField("Negative Color :", negativeColor);

		positiveImage = EditorGUILayout.ObjectField(
			"Image for Positive Bar :",
			positiveImage,
			typeof(Sprite),
			true) as Sprite;
		negativeImage = EditorGUILayout.ObjectField(
			"Image for Positive Bar :",
			negativeImage,
			typeof(Sprite),
			true) as Sprite;
	}

	private FloatVariable CreateNewFloatVariable()
	{
		string path = "Assets/FloatingBars";
		FloatVariable newFloatVariable = CreateInstance("FloatVariable") as FloatVariable;
		newFloatVariable.name = floatVariableName;
		newFloatVariable.SetValue(floatVariableValue);

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + floatVariableName + ".asset");
		AssetDatabase.CreateAsset(newFloatVariable, assetPathAndName);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		return newFloatVariable;
	}
}
