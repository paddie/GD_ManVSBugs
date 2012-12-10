var firing : boolean = false;
private var muzzleFlashGenerator : Transform;
private var muzzleFlashGeneratorScript : muzzleFlashGenerator;

function Start(){
	muzzleFlashGenerator = transform.Find("muzzleFlashGenerator");
	muzzleFlashGeneratorScript = muzzleFlashGenerator.GetComponent("muzzleFlashGenerator");
	firing = false;
}

function Update () {
	muzzleFlashGeneratorScript.on = firing;
}

function Fire() {
	firing = true;
}

function StopFiring() {
	firing = false;
}