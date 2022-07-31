using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardClass : MonoBehaviour
{
    private Sprite foregroundTexture, backgroundTexture;
    [System.NonSerialized]
    public int matchId;
    private Image image;
    private bool flipState;
    private RectTransform rect;
    private bool pointerLock;
    private bool flipDirection;
    private bool isController = false;
    private bool lockedFromController = false;
    private Coroutine flipCoroutine, fadeCoroutine;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    IEnumerator FlipCoroutine(){
        while(pointerLock){
            if (flipDirection)
            {
                GameController.Instance.globalLock = true;
                rect.rotation = Quaternion.Lerp(rect.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime * GameController.Instance.cardFlipSpeed);
                if (rect.rotation.eulerAngles.y > 89.5f)
                {
                        rect.rotation = Quaternion.Euler(0, 90, 0);
                        updateTexture();
                        flipDirection = false;
                }
            }
            else
            {
                rect.rotation = Quaternion.Lerp(rect.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * GameController.Instance.cardFlipSpeed);
                if (rect.rotation.eulerAngles.y < 0.5f)
                {
                    rect.rotation = Quaternion.Euler(0, 0, 0);
                    pointerLock = false;
                    GameController.Instance.globalLock = false;
                    if (!isController)
                        GameController.Instance.addIdForMatch(this);
                }
            }
            yield return null;
        }
        yield break;
    }

    IEnumerator FadeCoroutine(){
        while(image.canvasRenderer.GetAlpha() != 0.0f){
            image.CrossFadeAlpha(0, 0.1f, false);
            yield return null;
        }
        Lock();
        yield break;
    }

    public void onPointerDown()
    {
        if (!lockedFromController && !pointerLock && !GameController.Instance.globalLock)
            flipTheCard();
    }

    public void Lock()
    {
        lockedFromController = true;
    }

    public void Unlock()
    {
        lockedFromController = false;
    }
    public void SetValues(Sprite foregroundTexture, int matchId, Sprite backgroundTexture)
    {
        this.foregroundTexture = foregroundTexture;
        this.matchId = matchId;
        this.backgroundTexture = backgroundTexture;
        flipState = false;
        pointerLock = false;
        image = gameObject.GetComponent<Image>();
        setTexture(this.backgroundTexture);
        Unlock();
    }

    public void setTexture(Sprite newTexture)
    {
        image.sprite = newTexture;
    }

    public void updateTexture()
    {
        image.sprite = (flipState) ? foregroundTexture : backgroundTexture;
    }

    public void flipTheCard(bool isController = false)
    {
        this.isController = isController;
        flipState = !flipState;
        pointerLock = true;
        flipDirection = true;
        flipCoroutine = StartCoroutine(FlipCoroutine());
    }

    public void onMatch(){
        fadeCoroutine = StartCoroutine(FadeCoroutine());
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void SetLocalScale(Vector3 vector)
    {
        gameObject.GetComponent<RectTransform>().localScale = vector;
    }
}
