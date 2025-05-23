using UnityEngine;

public class BirdFlocking : MonoBehaviour
{
    //if player is far enough away
    //activate birds off screen
    //fly into the flock point
    //change the bird's state to hopping around
    //if the player steps in the flock collider
    //birds switch to flying state
    //once flown away deactivate birds

    //birds pick a point within a certain range of the object
    //birds lerp towards that point
    //birds choose a random direction and hop towards it
    //how do birds hop
    //choose a distance and direction
    //looks at target direction
    //moves horizontally to direction by distance
    //add a slight upwards force that then goes back down??
    //i dont want to use built in physics for this just basic movements
    //could just get a vertical offset value say like tranform.position.y +0.5f
    //lerp towards that position then lerp back down to original position 
}
