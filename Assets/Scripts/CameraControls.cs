using UnityEngine;

public class CameraControls : MonoBehaviour
{
    // Add this to any script where you want to start the comparison
    public AlgorithmVisualizer visualizer;

    //Speed of camera controls
    private const float Speed = 8f;
    
    
    //Camera movement function
    void MovePlayer()
    {
        //Get horizontal keyboard input
        float LeftRight = Input.GetAxis("Horizontal");
        //Create and store movement in Vector3 & multiply by speed and time
        Vector3 moveX = new Vector3(LeftRight, 0, 0) * Time.deltaTime * Speed;
        //Translates horizontal keyboard input into sidetoside cam movement
        transform.Translate(moveX, Space.Self);


        //Get vertical keyboard input
        float ForwardBack = Input.GetAxis("Vertical");
        //Create and store movement in Vector3 & multiply by speed and time
        Vector3 moveZ = new Vector3(0, 0, ForwardBack) * Time.deltaTime * Speed;
        //Translates vertical keyboard input into forward cam movement
        transform.Translate(moveZ, Space.Self);


        //Create variable to store Up & Down keyboard input
        float UpDown = 0;
        //If Q key pressed then camera moves down
        if (Input.GetKey(KeyCode.Q)) UpDown = -1;
        //If E key pressed then camera moves up
        if (Input.GetKey(KeyCode.E)) UpDown = 1;
        //Create and store movement in Vector3 & multiply by speed and time
        Vector3 moveY = new Vector3(0, UpDown, 0) * Time.deltaTime * Speed;
        //Translates Q & E keyboard input into UP/DOWN cam movement
        transform.Translate(moveY, Space.Self);

        //Create variable to store Rotate left/right keyboard input
        float rotateLeftRight = 0;
        //If Z key pressed then camera looks left
        if (Input.GetKey(KeyCode.Z)) rotateLeftRight = -3;
        //If X key pressed then camera looks right
        if (Input.GetKey(KeyCode.X)) rotateLeftRight = 3;
        //Create and store movement in Vector3 & multiply by speed and time
        Vector3 moveRotY = new Vector3(0, rotateLeftRight, 0) * Time.deltaTime * Speed;
        //Translates Z & X keyboard input into looking left/right cam movement
        transform.Rotate(moveRotY, Space.Self);

        //Create variable to store Rotate up/down keyboard input
        float rotateUpDown = 0;
        //If R key pressed then camera looks up
        if (Input.GetKey(KeyCode.R)) rotateUpDown = -3;
        //If F key pressed then camera looks down
        if (Input.GetKey(KeyCode.F)) rotateUpDown = 3;
        //Create and store movement in Vector3 & multiply by speed and time
        Vector3 moveRotX = new Vector3(rotateUpDown, 0, 0) * Time.deltaTime * Speed;
        //Translates Z & X keyboard input into looking up/down cam movement
        transform.Rotate(moveRotX, Space.Self);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();   
    }
}
