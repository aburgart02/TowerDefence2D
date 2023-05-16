using UnityEngine;

namespace DefaultNamespace
{
    public class Hammer : MonoBehaviour
    {
        private SpriteRenderer hammer;
        private Sprite sprite;
        public bool isActive;
        private Vector3 position;
        
        public void Start()
        {
            hammer = GetComponent<SpriteRenderer>();
            hammer.enabled = false;
            position = hammer.transform.position;
        }

        public void Update()
        {
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
                        hit.collider.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                        hit.collider.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        hit.collider.gameObject.GetComponent<Tower>().isDestroyed = true;
                        DisableDragSprite();
                    }
                }
            }
        
            if (hammer.enabled)
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
            hammer.enabled = true;
            hammer.sprite = sprite;
            hammer.sortingOrder = 10;
        }

        public void DisableDragSprite()
        {
            hammer.enabled = false;
            isActive = false;
            hammer.transform.position = position;
        }
    
        public void SelectedTower()
        {
            EnableDragSprite();
        }
    }
}