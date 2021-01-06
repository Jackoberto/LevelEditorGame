using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Editor : MonoBehaviour {
	
	public string tileFolder;
	[HideInInspector] public List<Tile> tile;
    [HideInInspector] public List<string> tileName;
    [HideInInspector] public List<string> editorTileNames = new List<string>();
    public GameObject mouseTile, defaultImage, tilesMaster;
    public Text placeHolderText;
    public EditorCamera cam;
	public SaveEdit edit;
	public float scrollSens;
	public InputField currentTileInputField;
	private List<Image> _tileButtons = new List<Image>();
	private SpriteRenderer _rend;
	[HideInInspector] public Tilemap map;
	private Grid _grid;
	private int _tileNum;
	private Vector3Int _lastPosition;
	private bool _tileMode;
	
	public UserTileData TileData { get; set; }

	private bool ValidMousePosition(Vector3 mousePosScreen)
	{
		return !(mousePosScreen.x > 1) && !(mousePosScreen.x < 0) && !(mousePosScreen.y > 1) && !(mousePosScreen.y < 0);
	}

	// Use this for initialization
	void Start ()
	{
		InitializeFields();
		InitButtons();
		SetTileMap();
	}

	private void InitButtons()
	{
		var previousSave= SetTileData();
		for (var i = 0; i < tile.Count; i++)
		{
			tileName.Add(tile[i].name);
			if (!previousSave)
				editorTileNames.Add(tile[i].name);
			var instance = Instantiate(defaultImage, tilesMaster.transform);
			var image = instance.GetComponent<Image>();
			_tileButtons.Add(image);
			image.sprite = tile[i].sprite;
			image.color = tile[i].color;
			var temp = i;
			instance.GetComponent<Button>().onClick.AddListener(() => { ChooseTile(temp); });
			instance.GetComponent<TileSelectButton>().onRightClick.AddListener(() => { ChooseColor(temp); });
			instance.name = editorTileNames[i];
		}
		ChooseTile(0);
	}

	private void AddTileAtStart()
	{
		var obj = ScriptableObject.CreateInstance<Tile>();
		obj.name = "NewTile" + tile.Count;
		obj.sprite = tile[0].sprite;
		tile.Add(obj);
		editorTileNames.Add("NewTile" + tile.Count);
	}
	
	public void AddTile()
	{
		AddTileAtStart();
		tileName.Add(tile[tile.Count - 1].name);
		var instance = Instantiate(defaultImage, tilesMaster.transform);
		var image = instance.GetComponent<Image>();
		_tileButtons.Add(image);
		image.sprite = tile[tile.Count - 1].sprite;
		image.color = tile[tile.Count - 1].color;
		var temp = tile.Count - 1;
		instance.GetComponent<Button>().onClick.AddListener(() => { ChooseTile(temp); });
		instance.GetComponent<TileSelectButton>().onRightClick.AddListener(() => { ChooseColor(temp); });
		instance.name = editorTileNames[tile.Count - 1];
		ChooseTile(tile.Count - 1);
	}

	private void InitializeFields()
	{
		_grid = GetComponentInChildren<Grid>();
		map = GetComponentInChildren<Tilemap>();
		_rend = mouseTile.GetComponent<SpriteRenderer>();
		_tileMode = true;
		tile = Resources.LoadAll<Tile>(tileFolder).ToList();
		tileName = new List<string>();
	}

	private bool SetTileData()
	{
		if (TileData == null) return false;
		while (tile.Count < TileData.colors.Count)
		{
			AddTileAtStart();
		}
		editorTileNames = TileData.editorNames;
		for (var i = 0; i < tile.Count; i++)
		{
			tile[i].color = TileData.colors[i].GetUnityColor();
		}
		map.RefreshAllTiles();
		return true;
	}

	private void SetTileMap()
	{
		var saveEdit = FindObjectOfType<SaveEdit>();
		if (saveEdit.MapData == null) return;
		saveEdit.LoadTileMap(this);
	}

	// Update is called once per frame
	void Update()
	{
		TilePlacing();
    }

	private void TilePlacing()
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
		var position = _grid.WorldToCell(worldPoint);
        var overUI = IsPointerOverUI();
		/*if (scrollbar.handleRect.anchoredPosition.y > 532)
        {
			scrollbar.handleRect.anchoredPosition = new Vector2(0, 532);
		}

		if (scrollbar.handleRect.anchoredPosition.y < 0)
		{
			scrollbar.handleRect.anchoredPosition = new Vector2(0, 0);
		}*/
		_tileMode = ValidMousePosition(Camera.main.ScreenToViewportPoint(Input.mousePosition));

		if (_tileMode)
		{

			if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftShift) && !overUI)
			{
				print(position);
				int xDiff = position.x - _lastPosition.x;
				int yDiff = position.y - _lastPosition.y;
				print("xDiff" + xDiff + "yDiff " + yDiff);
				if (xDiff < 0 && yDiff < 0)
				{
					map.BoxFill(position, tile[_tileNum], position.x, position.y, _lastPosition.x, _lastPosition.y);
				}

				else if (xDiff > 0 && yDiff > 0)
				{
					map.BoxFill(position, tile[_tileNum], _lastPosition.x, _lastPosition.y, position.x, position.y);
				}

				else if (xDiff > 0 && yDiff < 0)
				{
					map.BoxFill(position, tile[_tileNum], _lastPosition.x, position.y, position.x, _lastPosition.y);
				}

				else if (xDiff < 0 && yDiff > 0)
				{
					map.BoxFill(position, tile[_tileNum], position.x, _lastPosition.y, _lastPosition.x, position.y);
				}

				else if (xDiff > 0 && yDiff == 0)
				{
					for (int i = 0; i < xDiff + 1; i++)
					{
						map.SetTile(new Vector3Int(_lastPosition.x + i, _lastPosition.y, _lastPosition.z), tile[_tileNum]);
					}
				}

				else if (xDiff < 0 && yDiff == 0)
				{
					for (int i = 0; i > xDiff - 1; i--)
					{
						map.SetTile(new Vector3Int(_lastPosition.x + i, _lastPosition.y, _lastPosition.z), tile[_tileNum]);
					}
				}

				if (yDiff > 0 && xDiff == 0)
				{
					for (int i = 0; i < yDiff + 1; i++)
					{
						map.SetTile(new Vector3Int(_lastPosition.x, _lastPosition.y + i, _lastPosition.z), tile[_tileNum]);
					}
				}

				else if (yDiff < 0 && xDiff == 0)
				{
					for (int i = 0; i > yDiff - 1; i--)
					{
						map.SetTile(new Vector3Int(_lastPosition.x, _lastPosition.y + i, _lastPosition.z), tile[_tileNum]);
					}
				}

				_lastPosition = position;

			}


			else if (Input.GetKey(KeyCode.Mouse0) && !overUI)
			{
				print(position);
				map.SetTile(position, tile[_tileNum]);
				_lastPosition = position;
				TileBase type = map.GetTile(position);
				print(type);
			}

			if (Input.GetKey(KeyCode.Mouse1) && !overUI)
			{
				map.SetTile(position, null);
			}

			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				_tileNum++;
				if (_tileNum >= tile.Count)
				{
					_tileNum = 0;
				}
			}

			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				_tileNum--;
				if (_tileNum <= -1)
				{
					_tileNum = tile.Count - 1;
				}
			}

			if (!overUI)
			{
				mouseTile.transform.position = position + new Vector3(0.5f, 0.5f, 0);
				_rend.enabled = true;
			}

			else
			{
				_rend.enabled = false;
			}
		}
		//img.mouseTile = tile[_tileNum].mouseTile;
		_rend.sprite = tile[_tileNum].sprite;
		var color = tile[_tileNum].color;
		color.a = 120/255f;
		_rend.color = color;

		//Vector3 mousePos = Input.mousePosition - _rect.localPosition;
        //_imgRect.localPosition = new Vector3(mousePos.x, mousePos.y, mousePos.z);
	}

	private void BoxFill()
	{
		
	}

	private bool IsPointerOverUI()
    {
	    var eventSystem = EventSystem.current;
	    return eventSystem.IsPointerOverGameObject();
    }

	public void ChangeTileName(string newName)
	{
		editorTileNames[_tileNum] = newName;
	}

	private void ChooseTile(int chosenTile)
    {
	    _tileNum = chosenTile;
	    if (currentTileInputField != null)
		    currentTileInputField.SetTextWithoutNotify("");
	    if (placeHolderText != null)
		    placeHolderText.text = editorTileNames[_tileNum];
    }

	private void ChooseColor(int chosenTile)
	{
		ColorPicker.Create(tile[chosenTile].color, "Choose Tile Color",
			delegate(Color color) { SetTileColor(chosenTile, color); }, delegate(Color color) { SetTileColor(chosenTile, color); });
	}
	public void Save()
	{
		edit.Save(new UserTileData(tile, editorTileNames));
	}
	private void SetTileColor(int chosenTile, Color colorToChangeTo)
	{
		tile[chosenTile].color = colorToChangeTo;
		map.RefreshAllTiles();
		RefreshTileButtons();
	}
	

	private void SetTileColor(Tile tileToChange, Color colorToChangeTo)
	{
		tileToChange.color = colorToChangeTo;
		map.RefreshAllTiles();
		RefreshTileButtons();
	}

	private void RefreshTileButtons()
	{
		for (var i = 0; i < tile.Count; i++)
		{
			_tileButtons[i].color = tile[i].color;
		}
	}

	/*public void ScrollSens(float value)
	{
	    scrollSens = value;
	}

	public void ScrollBar(float value)
	{
	    tilesMaster.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -300 + (-15250 * value));
		scrollbar.handleRect.anchoredPosition = new Vector2(0, 0);
	}

	public void SwitchCategory(int num)
	{
		if (num == 0)
	    {
			tilesMaster.SetActive(true);
			characterMaster.SetActive(false);
			_tileMode = true;
	    }	

		if (num == 1)
	    {
			tilesMaster.SetActive(false);
			characterMaster.SetActive(true);
			_tileMode = false;
	    }
	}*/
}
