using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public TowerButton TowerButtonPressed { get; set; }
    private SpriteRenderer spriteRenderer;
    private readonly List<Tower> towerList = new();
    private readonly List<Collider2D> buildList = new();
    private Collider2D buildTile;
    public GameManager gameManager;
    public SoundManager soundManager;
    
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if(hit.collider.CompareTag("buildSite"))
            {
                buildTile = hit.collider;
                buildTile.tag = "buildSiteFull";
                RegisterBuildSite(buildTile);
                PlaceTower(hit);
            }
        }
        
        if (spriteRenderer.enabled)
        {
            FollowMouse();
        }
    }

    private void RegisterBuildSite(Collider2D buildTag)
    {
        buildList.Add(buildTag);
    }

    private void RegisterTower(Tower tower)
    {
        towerList.Add(tower);
    }

    public void RenameTagsBuildSites()
    {
        foreach(Collider2D buildTag in buildList)
        {
            buildTag.tag = "buildSite";
        }
        buildList.Clear();
    }

    public void DestroyAllTower()
    {
        foreach(Tower tower in towerList)
        {
            Destroy(tower.gameObject);
        }
        towerList.Clear();
    }

    private void PlaceTower(RaycastHit2D hit)
    {
        if (!EventSystem.current.IsPointerOverGameObject() && TowerButtonPressed != null)
        {
            Tower newTower = Instantiate(TowerButtonPressed.TowerObject);
            newTower.transform.position = hit.transform.position;
            BuyTower(TowerButtonPressed.TowerPrice);
            gameManager.AudioSource.PlayOneShot(soundManager.TowerBuiltClip);
            RegisterTower(newTower);
            DisableDragSprite();
        }
    }

    private void BuyTower(int price)
    {
        gameManager.SubtractMoney(price);
    }
    
    public void SelectedTower(TowerButton towerSelected)
    {
        if (towerSelected.TowerPrice <= gameManager.currentMoney)
        {
            TowerButtonPressed = towerSelected;
            EnableDragSprite(towerSelected.DragSprite);
        }
    }

    private void FollowMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    private void EnableDragSprite(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;
    }

    public void DisableDragSprite()
    {
        spriteRenderer.enabled = false;
    }
}
