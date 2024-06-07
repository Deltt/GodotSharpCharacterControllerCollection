![demo](https://github.com/Deltt/GodotSharpCharacterControllerCollection/assets/70781525/b6e50652-ba82-492c-a1a7-8418e46aa18d)

Hello fine people, I want to share my small collection of various C# character controllers, hoping some people find them as handy as I do :)

These aren't optimized by any means, but they don't do anything beyond simple camera and character controls.

What are these for?
For quickly testing and prototyping mechanics, or simply exploring levels and environments.

How do I use them?
Copy the folder of the character controller you want into your project folder, then drag the X_character.tscn into your desired scene. No setup needed! (strategy_camera.tscn for stragegy)

Note: You may need to rebuild the C# solution for Godot to recognize copied-in scripts, see the text file inside the download folder for more info)

They all have a test_environment scene for you to test them out quickly.

What are the controls?
The controls are for the most part what you would expect them to be based on the genre of the character. The exception for this is the StrategyCamera which zooms with Q and E (because fsr mousewheel wouldn't get recognized). They are hardcoded so no need to setup Input Actions. Escape will quit the game.

Controllers are supported for FirstPerson, ThirdPerson, and ThirdPersonAction (as JoyPad Device 0).

Customization
While these only are meant to be used for testing/prototyping, there's still a bunch of options and parameters you can set to get a more accurate feel of what you want in your own controller later.

Many exported parameters for adjusting (otherwise feel free to edit the script)

Camera Sway option for ThirdPersonCamera (like many platformers have, enabled by default)

Never Face Backwards option for ThirdPersonActionCharacter (will make the character backpaddle instead of rotating towards the camera when running backwards, enabled by default)
