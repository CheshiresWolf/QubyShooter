using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;

public class FileBrowser : MonoBehaviour {

	public GameObject window;
	public Button okButton;
	public InputField inputField;
	public GameObject scrollview;
	public GameObject scrollParent;

	public GameObject icosaedron;
	
	bool mode = false;
	string levelsDir = "Assets/levels/";

	Text okButton_text;

	bool saveOpened = false;
	bool loadOpened = false;

	List<GameObject> buttons = new List<GameObject>();

	delegate void myButtonListener(string name);

	public void openSave() {
		if (loadOpened) {
			closeWindow ();
		}

		if (saveOpened) {
			closeWindow ();
		} else {
			saveOpened = true;

			okButton_text.text = "Save";
			window.SetActive(true);

			//Vector2 scrollSize = scrollview.GetComponent<RectTransform>().sizeDelta;
			//Rect scrollRect = scrollview.GetComponent<RectTransform> ().rect;
			//Vector2 scrollVector = scrollview.GetComponent<RectTransform> ().sizeDelta;
			
			FileInfo[] files = getDirList();
			for (int i = 0; i < files.Length; i++) {
				buttons.Add(createButton(
					"file_button" + i,
					files [i].Name,
					scrollview.transform,
					new Vector2 (300.0f, 30.0f),
					saveListClickEvent
				));
			}

			okButton.onClick.RemoveAllListeners();
			okButton.onClick.AddListener(() => buttonListener("save"));
		}
	}
	
	public void openLoad() {
		if (saveOpened) {
			closeWindow ();
		}
		
		if (loadOpened) {
			closeWindow ();
		} else {
			loadOpened = true;

			inputField.GetComponent<InputField>().interactable = false;

			okButton_text.text = "Load";
			window.SetActive (true);

			FileInfo[] files = getDirList ();
			for (int i = 0; i < files.Length; i++) {
				buttons.Add (createButton (
					"file_button" + i,
					files [i].Name,
					scrollview.transform,
					new Vector2 (300.0f, 30.0f),
					loadListClickEvent
				));
			}
			
			okButton.onClick.RemoveAllListeners();
			okButton.onClick.AddListener(() => buttonListener("load"));
		}
	}

	public void buttonListener(string operation) {
		string selectedFileName = inputField.text;

		if (selectedFileName != "") {
			if (operation == "save") {
				icosaedron.GetComponent<Generator>().saveToFile(selectedFileName);
			} else {
				icosaedron.GetComponent<Generator>().loadFromFile(selectedFileName);
			}

			closeWindow();
		}
	}

	public void closeWindow() {
		window.SetActive(false);
		buttons.ForEach(child => Destroy(child));

		inputField.text = "";
		inputField.GetComponent<InputField>().interactable = true;
		
		saveOpened = false;
		loadOpened = false;
	}

	FileInfo[] getDirList() {
		DirectoryInfo dir = new DirectoryInfo(levelsDir);
		FileInfo[] info = dir.GetFiles("*.txt");

		return info;
	}

	GameObject createButton(string name, string filename, Transform panel, Vector2 size, myButtonListener callback) {//Vector3 position, UnityEngine.Events.UnityAction method) {
		GameObject button = new GameObject();
		RectTransform rt = button.AddComponent<RectTransform>();
		Button rb = button.AddComponent<Button>();

		GameObject buttonText = new GameObject();
		Text text = buttonText.AddComponent<Text>();
		buttonText.GetComponent<RectTransform>().sizeDelta = size;
		Font font = Resources.LoadAssetAtPath<Font>("Assets/fonts/CloisterBlack.ttf");

		text.name = "text";
		text.font = font;
		text.fontSize = 14;
		text.color = Color.black;
		text.alignment = TextAnchor.MiddleLeft;//TextAlignment.Center;
		text.text = filename;
		buttonText.transform.SetParent(button.transform);

		button.name = name;
		button.transform.SetParent(panel);

		rt.sizeDelta = size;
		rb.onClick.AddListener(() => callback(filename));

		return button;
	}

	void saveListClickEvent(string text) {
		Debug.Log("FileBrowser | saveListClickEvent | text : " + text);
		inputField.text = text;
	}

	void loadListClickEvent(string text) {
		Debug.Log("FileBrowser | loadListClickEvent | text : " + text);
		inputField.text = text;
	}
	
	// Use this for initialization
	void Start () {
		okButton_text = okButton.GetComponentInChildren<Text>();
		window.SetActive(false);
		//scrollview    = window.transform.FindChild("Scrollview").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
