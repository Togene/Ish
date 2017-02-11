using UnityEngine;
using System.Collections;

public class Ish_Animation_Manager : MonoBehaviour {

	public Animator anim; //<--- this dude total Hash128 an animator
	public Kyra_Rules kyraRules;
	public int numberofPeeps;
	public bool isWalking, isRight, isLeft, isUp, isDown;



	void Start () 
	{
		anim = GetComponent<Animator>();

		//Telling the Animator some shit
		anim.SetBool("Walking", false);



		isWalking = false;
		kyraRules = GetComponent<Kyra_Rules>();

		//kyraRules.isKyraGay = true;
	}

	void Update () 
	{

		if(Input.GetKey("w"))
		{
			anim.SetBool("Up", true);
		}

		if(Input.GetKey("a"))
		{
			anim.SetBool("Left", true);
		}

		if(Input.GetKey("d"))
		{
			anim.SetBool("Right", true);
		}

		if(Input.GetKey("s"))
		{
			anim.SetBool("Down", true);
		}

		numberofPeeps ++;
	}
}
