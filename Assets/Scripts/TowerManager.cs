using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;

public class TowerManager : MonoBehaviour
{
    public TowerButton TowerButtonPressed { get; set; }
    private SpriteRenderer spriteRenderer;
    public List<Tower> towerList = new();
    public GameManager gameManager;
    public SoundManager soundManager;

    void Start () 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    
	void Update ()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DisableDragSprite();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hit = Physics2D.RaycastAll(worldPoint, Vector2.zero);

            if (hit.Length > 0)
            {
                if (hit.Any(x => x.collider.CompareTag("buildSite")) 
                    && !hit.Any(x => x.collider.CompareTag("Tower")))
                {
                    PlaceTower(hit.First());
                }
            }
        }
        
        if (spriteRenderer.enabled)
        {
            FollowMouse();
        }
    }

    private void RegisterTower(Tower tower)
    {
        towerList.Add(tower);
    }

    public void DestroyAllTower()
    {
        foreach(Tower tower in towerList)
        {
            if (tower != null)
                Destroy(tower.gameObject);
        }
        towerList.Clear();
    }

    private void PlaceTower(RaycastHit2D hit)
    {
        if (!EventSystem.current.IsPointerOverGameObject() && TowerButtonPressed != null 
                                                           && TowerButtonPressed.TowerPrice <= gameManager.currentMoney)
        {
            Tower newTower = Instantiate(TowerButtonPressed.TowerObject);
            newTower.transform.position = hit.transform.position;
            BuyTower(TowerButtonPressed.TowerPrice);
            gameManager.AudioSource.PlayOneShot(soundManager.TowerBuiltClip);
            RegisterTower(newTower);
            TowerButtonPressed = null;
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
