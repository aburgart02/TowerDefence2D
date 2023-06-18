using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UpgradeTool : MonoBehaviour
    {
        private SpriteRenderer upgradeTool;
        private Sprite sprite;
        public bool isActive;
        private Vector3 position;
        public GameManager gameManager;
        public Text t;
        
        public void Start()
        {
            upgradeTool = GetComponent<SpriteRenderer>();
            upgradeTool.enabled = false;
            position = upgradeTool.transform.position;
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        public void Update()
        {
            Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D h = Physics2D.Raycast(wp, Vector2.zero);

            if (h.collider)
            {
                if (h.collider.CompareTag("Tower") && isActive)
                {
                    var twr = h.collider.gameObject.GetComponent<Tower>();
                    if (twr.towerLevel == 1)
                        t.text = Convert.ToString(twr.upgradePrice) + "$";
                }
                
                if (h.collider.CompareTag("BabkinaSarayka") && isActive)
                {
                    var babkinaSarayka = h.collider.gameObject.GetComponent<BabkinaSarayka>();
                    if (!babkinaSarayka.isActive)
                        t.text = "300$";
                }
            }
            else
            {
                t.text = "";
            }

            if (Input.GetMouseButtonDown(1))
            {
                DisableDragSprite();
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                       
                if (hit.collider)
                {
                    if (hit.collider.CompareTag("Tower") && isActive)
                    {
                        var tower = hit.collider.gameObject.GetComponent<Tower>();
                        if (tower.towerType == "ArrowTower" && tower.towerLevel < 2 && tower.upgradePrice <= gameManager.currentMoney)
                        {
                            gameManager.SubtractMoney(tower.upgradePrice);
                            tower.towerLevel += 1;
                            tower.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("a2");
                        }
                        if (tower.towerType == "RockTower" && tower.towerLevel < 2 && tower.upgradePrice <= gameManager.currentMoney)
                        {
                            gameManager.SubtractMoney(tower.upgradePrice);
                            tower.towerLevel += 1;
                            tower.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("b2");
                        }
                        if (tower.towerType == "FireballTower" && tower.towerLevel < 2 && tower.upgradePrice <= gameManager.currentMoney)
                        {
                            gameManager.SubtractMoney(tower.upgradePrice);
                            tower.towerLevel += 1;
                            tower.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("c2");
                        }

                        DisableDragSprite();
                    }
                    
                    if (hit.collider.CompareTag("BabkinaSarayka") && isActive)
                    {
                        var babkinaSarayka = hit.collider.gameObject.GetComponent<BabkinaSarayka>();
                        if (300 <= gameManager.currentMoney)
                        {
                            babkinaSarayka.isActive = true;
                            gameManager.SubtractMoney(300);
                        }
                        
                        DisableDragSprite();
                    }
                }
            }
        
            if (upgradeTool.enabled)
            {
                FollowMouse();
            }
        }
        
        private void FollowMouse()
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    
        private void EnableDragSprite()
        {
            isActive = true;
            upgradeTool.enabled = true;
            upgradeTool.sprite = sprite;
            upgradeTool.sortingOrder = 10;
        }

        public void DisableDragSprite()
        {
            upgradeTool.enabled = false;
            isActive = false;
            upgradeTool.transform.position = position;
        }
    
        public void SelectedTower()
        {
            EnableDragSprite();
        }
    }
}