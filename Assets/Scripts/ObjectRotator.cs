using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    //a script to rotate and float any object it's on
    [Header("References")]
    [SerializeField] private GameObject rotated;
    [SerializeField] private float speed;


    private void Awake()
    {
        if(rotated == null)
            return; 
        rotated.SetActive(false);
        
    }

    private void Update()
    {
        if (rotated == null) return;
        if (!rotated == gameObject)
        {
            rotated = gameObject; 
        }
    }





}
