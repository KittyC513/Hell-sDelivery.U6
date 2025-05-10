using UnityEngine;

public class PlayerBlobShadow : MonoBehaviour
{
    [SerializeField] private GameObject shadowObj; //the object that actually gets put on the ground, this object should be a parent to the object with the sprite
    [SerializeField] private float raycastDistance = 20; //how far down the raycast will detect ground
    [SerializeField] private GameObject player; //the main player object

    [SerializeField] private LayerMask groundMask; //the layers we want to cast a shadow onto

    private Vector3 shadowCastPoint; //the point where we should cast the shadow

    private RaycastHit groundHit; //raycast hit information based on the raycast

    private float distanceToGround = 0; //how far is the ground from the player

    private float hitAngle;

    [Space, Header("Scale Effect Values")]
    [SerializeField] private float distanceFactor = 2f; //the bigger this number is the less the distance affects the scale
    [SerializeField] private float minimumDistance = 1.2f; //what the distance value should be while grounded

    [Range(0.1f, 1.0f)]
    [SerializeField] private float minimumScale = 0.4f; //the smallest scale value that the shadow can be scaled by


    private void Awake()
    {
        player = this.gameObject;
    }
    private void LateUpdate()
    {
        //if ground was detected by our raycast
        if (FindGround())
        {
            //set the object to active
            shadowObj.SetActive(true); 

            //set the objects position to the point the raycast hit (plus a little upwards offset)
            shadowObj.transform.position = shadowCastPoint;

            UpdateBlobSize();
        }
        else
        {
            //otherwise set the object to inactive so it does not show
            shadowObj.SetActive(false);
        }
    }

    private bool FindGround()
    {
        Vector3 raycastStartPoint = player.transform.position;
        //shoot a raycast downwards looking for ground
        if (Physics.SphereCast(raycastStartPoint, 0.22f, Vector3.down, out RaycastHit hit, raycastDistance, groundMask))
        {
            RaycastHit targetHit = hit;

            //set our target point to slightly above the hit point, this is where we will display the shadow
            Vector3 targetPoint = new Vector3(hit.point.x, hit.point.y + 0.05f, hit.point.z);
            
            groundHit = targetHit;

            distanceToGround = targetHit.distance;

            shadowCastPoint = targetPoint;

            return true;
        }

        return false;
    }

    private void UpdateBlobSize()
    {
        //lerp the scale value between the minimum scale and 1, at minimum distance to ground the scale is 1 and as you get farther away the scale value gets smaller
        float scaleValue = Mathf.Lerp(minimumScale, 1, (minimumDistance / distanceToGround) * distanceFactor);
        shadowObj.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
    }
}
