exec("./attributes.cs");
//to do: EnvGuiServer? should this be renamed?
//to do: replace client-sided stuff

package Support_Better_Environment_Server
{
	function serverCmdEnvGui_SetVar(%client,%var,%a,%b)
	{
		switch$(%var)
		{
			case "WaterHeight":
				if(%a $= "Inf")
					EnvGuiServer::SetInfiniteWater();
				else
					if($EnvGuiServer::WaterInfinite && !$EnvGuiServer::SimpleMode) //dedi warning: test this
						return;
		}
		Parent::serverCmdEnvGui_SetVar(%client,%var,%a,%b);
	}
};

function EnvGuiServer::SetInfiniteWater(%client,%a)
{
	if(%a)
	{
		waterZone.setScale("1e+006 1e+006 1e+006"); //1e+006 1e+006 1e+006 (?)
		waterPlane.setTransform("0 0 1e+006");
		waterPlane.setScale(waterPlane.scale); //?
		$EnvGuiServer::WaterInfinite = 1;
	}
	else
	{
		//undo the infinite water
		waterZone.setScale("1e+006 1e+006 100");
		waterPlane.setTransform("0 0 " @ $EnvGuiServer::WaterHeight-0.5); //dedi warning: test this
		waterPlane.setScale(waterPlane.scale); //?
		$EnvGuiServer::WaterInfinite = 0;
	}
}

//WIP
function EnvGuiServer::SetInvisibleFog(%client,%a) //not working
{
	if(%a) //Disable fog
	{
		Sky.fogColor = "0 0 0 0";
		Sky.fogVolumeColor1 = "0 0 0 0";
		Sky.fogVolumeColor2 = "0 0 0 0";
		Sky.fogVolumeColor3 = "0 0 0 0";
	}
	else
	{
		echo(notimplemented);
	}
}

deactivatePackage("Support_Better_Environment_Server");
activatePackage("Support_Better_Environment_Server");