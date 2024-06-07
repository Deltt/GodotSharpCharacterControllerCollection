![demo](https://github.com/Deltt/GodotSharpCharacterControllerCollection/assets/70781525/b6e50652-ba82-492c-a1a7-8418e46aa18d)

# 3D C# Character Controllers

These aren't optimized by any means, but they don't do anything beyond simple camera and character controls.


## What are these for?
For quickly testing and prototyping mechanics, or simply exploring levels and environments.


## How do I use them?
First, download or clone this repo.

If downloaded, import it inside of the Godot launcher.
If cloned, scan the folder containing the repo folder inside of the Godot launcher.

Now you can freely test the available controllers inside of the repo project (open and run test_environment.tscn scene) or copy specific controllers into your own project and use them there.

**Note: You may need to rebuild the C# solution for your project to recognize copied-in scripts! You find this option under Project -> Tools -> C# -> Create C# Solution inside of the Godot editor**.


## What are the controls?
The controls are for the most part what you would expect them to be based on the genre of the character. The exception for this is the StrategyCamera which zooms with Q and E (because fsr mousewheel wouldn't get recognized). They are hardcoded so no need to setup Input Actions. **Escape will quit the game**.

Controllers are supported for *FirstPerson*, *ThirdPerson*, and *ThirdPersonAction* (as JoyPad Device 0).


## Customization
While these only are meant to be used for testing/prototyping, there's still a bunch of options and parameters you can set to get a more accurate feel of what you want in your own controller later.

- Many exported parameters for adjusting (otherwise feel free to edit the script)

- Camera Sway option for ThirdPersonCamera (like many platformers have, **enabled by default**)

- Never Face Backwards option for ThirdPersonActionCharacter (will make the character backpaddle instead of rotating towards the camera when running backwards, **enabled by default**)


## Notes

- ***The characters are not on collision layer 1, to prevent camera obstruction through the character.
Keep this in mind if you're prototyping mechanics with these assets***

- Only the ThirdPersonCamera and StrategyCamera can be used as a standalone camera asset,
others require the character to work
