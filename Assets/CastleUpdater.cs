using UnityEngine;

public class CastleUpdater : MonoBehaviour {

    public Sprite[] sprites;
    
    public void updateSprite(float health)
    {
        if(health > 800) //80-100
        {
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        } else if (health > 500) //50-80
        {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        } else if (health > 250) //25-50
        {
            GetComponent<SpriteRenderer>().sprite = sprites[2];
        } else if (health > 0) //0-25
        {
            GetComponent<SpriteRenderer>().sprite = sprites[3];
        } else if (health <= 0) //dead
        {
            GetComponent<SpriteRenderer>().sprite = sprites[4];
        }
    }
}
