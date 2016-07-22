//finish resetting (still not working at all)

if(isObject(EnvGui_WindowSetValue_Frame))
	EnvGui_WindowSetValue_Frame.delete();
exec("./MessageBoxEnvDlg.gui");
EnvGui_WindowSetValue_Frame.setVisible(0);

package Support_Better_Environment_Client
{
	function clientCmdEnvGui_SetVar(%var,%val,%test)
	{
		//echo("before" SPC $EnvGui::Var[%var]);
		if($EnvGui::ResetVar !$= "") //we need to record all the current values (except the one we're resetting) in order to retain them.
		{
			//echo("CURRENT VAL: " @ %val @ "; DEFAULT: " @ %val);
			$EnvGui::ResetCount++;
			$EnvGui::ResetVarSet[$EnvGui::ResetCount] = %var TAB %val; //record all variables so we can retain them 
			
			//echo("#" @ $EnvGui::ResetCount @ ": adding " @ %var @ "(" @ %currentVal @ ")");
		}
	
		parent::clientCmdEnvGui_SetVar(%var,%val);	
		
		if($EnvGui::ResetVar $= %var) // If it is the value we're looking for, set it to the default value.
			$EnvGui::ResetVarSet[$EnvGui::ResetCount] = %var TAB $EnvGui::Var[%var];
		
		//////Reset Finished/////
		if(%var $= "VignetteColor" && $EnvGui::ResetVar !$= "") // VignetteColor is the last value, meaning everything is reset.
		{
			$EnvGui::ResetVar = "";
			
			for(%i = 1; %i <= $EnvGui::ResetCount; %i++)
			{
				// Set the new variables
				commandToServer('EnvGui_SetVar',getField($EnvGui::ResetVarSet[%i],0),getField($EnvGui::ResetVarSet[%i],1));
			}
			
			EnvGui_WindowSetValue_Box.setValue(%val); //update the box
			//("EnvGui_" @ $EnvGui::ResetVar).setValue(%val); //update the slider (necessary?)
			
			deleteVariables("$EnvGui::Reset*");
		}
		
		$EnvGui::Var[%var] = %val;
		
		if(EnvGui_WindowSetValue_Frame.getValue() $= %var)
			EnvGui_WindowSetValue_Box.setValue(%val); // Update the setValue box.
		//echo("set oldVar to" SPC $EnvGui::Var[%var]);
	}
};

deactivatePackage("Support_Better_Environment_Client");
activatePackage("Support_Better_Environment_Client");

//this adds the commands to the sliders
EnvGui_SunAzimuth.command = "EnvGui::ClickSetValue(\"SunAzimuth\");";
EnvGui_SunElevation.command = "EnvGui::ClickSetValue(\"SunElevation\");";
EnvGui_DayOffset.command = "EnvGui::ClickSetValue(\"DayOffset\");";
EnvGui_SunFlareSize.command = "EnvGui::ClickSetValue(\"SunFlareSize\");";
EnvGui_VisibleDistance.command = "EnvGui::ClickSetValue(\"VisibleDistance\");";
EnvGui_FogDistance.command = "EnvGui::ClickSetValue(\"FogDistance\");";
EnvGui_WaterHeight.command = "EnvGui::ClickSetValue(\"WaterHeight\");";
EnvGui_WaterScrollX.command = "EnvGui::ClickSetValue(\"WaterScrollX\");";
EnvGui_WaterScrollY.command = "EnvGui::ClickSetValue(\"WaterScrollY\");";
EnvGui_GroundScrollX.command = "EnvGui::ClickSetValue(\"GroundScrollX\");";
EnvGui_GroundScrollY.command = "EnvGui::ClickSetValue(\"GroundScrollY\");";

function EnvGui::ClickSetValue(%var)
{
	%gui = "EnvGui_" @ %var;
	%val = %gui.getValue();
	%range = %gui.range;
	
	cancel(envGui.lastClickedSched);
	if(envGui.lastClicked $= %var)
	{
		EnvGui_WindowSetValue_Frame.setVisible(1);
		
		EnvGui_WindowSetValue_Frame.setValue(%var);
		EnvGui_WindowSetValue_Box.setValue(%val);
		
		EnvGui_WindowSetValue_Box.rangeMin = getWord(%range,0);
		EnvGui_WindowSetValue_Box.rangeMax = getWord(%range,1);
		
		EnvGui_WindowSetValue_Button.command = "EnvGui::SetValueOK();";
		envGui.lastClicked = 0;
		
	}
	else
	{
		envGui.lastClicked = %var;
		envGui.lastClickedSched = envGui.schedule(1000,setAttribute,lastClicked,0);
	}
}

function EnvGui::SetValueOK()
{
	%var = EnvGui_WindowSetValue_Frame.getValue();
	%val = EnvGui_WindowSetValue_Box.getValue();
	%gui = "EnvGui_" @ %var;
	
	commandToServer('EnvGui_SetVar',%var,%val);
	ClientCmdEnvGui_SetVar(%var,%val); //test
	
	EnvGui_WindowSetValue_Frame.setVisible(0);
	//%gui.setValue(%val);
}

function EnvGui::ClickSetValueReset()
{
	%var = EnvGui_WindowSetValue_Frame.getValue();
	%gui = "EnvGui_" @ %var;
	
	$EnvGui::ResetVar = %var; // Tell the packaged functions that we're resetting this value.
	$EnvGui::ResetVarGUI = %gui;
	
	commandToServer('EnvGui_RequestCurrentVars');
	EnvGui::ClickDefaults(EnvGui); // Start the reset.
}

function EnvGui::ClickSetValueRandom()
{
	%var = EnvGui_WindowSetValue_Frame.getValue();
	%gui = "EnvGui_" @ %var;
	
	%rangeMin = EnvGui_WindowSetValue_Box.rangeMin;
	%rangeMax = EnvGui_WindowSetValue_Box.rangeMax;
	
	%rand = getRandom(%rangeMin*1000,%rangeMax*1000);
	%val = %rand/1000;
	
	commandToServer('EnvGui_SetVar',%var,%val);
	EnvGui_WindowSetValue_Box.setValue(%val); //update the box
	%gui.setValue(%val); //update the slider. make sure this works.
}

function EnvGui::SetValueTextValidate()
{
	%text = EnvGui_WindowSetValue_Box.getValue();

	if(EnvGui_WindowSetValue_Box.rangeMin $= "" || EnvGui_WindowSetValue_Box.rangeMax $= "")
		return;
	
	if(%text < EnvGui_WindowSetValue_Box.rangeMin)
	{
		EnvGui_WindowSetValue_Box.setValue(EnvGui_WindowSetValue_Box.rangeMin);
		return;
	}
	if(%text > EnvGui_WindowSetValue_Box.rangeMax)
	{
		EnvGui_WindowSetValue_Box.setValue(EnvGui_WindowSetValue_Box.rangeMax);
		return;
	}
}