MecanimEffects
==============

Bind visual effects to Mecanim (Unity 3D animation system) states in the editor.

## What is Effect

We define effect here as a GameObject prefab. It can contain animated textures, particle emitters, audio sources, anything you want. Follow next few steps to create a sample effect.

1. Create an empty GameObject with Game Object > Create empty menu command.
2. In the Inspector window click Add Component button and select Effects > Particle system.
3. Drag that game object from Hierarchy to Project window to create emty prefab.
4. That's it!

## Using effects with Unity Editor

To try out mecanim effects you'll need a scene with any object animated with mecanim. Open that scene in Unity Editor and prepare the sample effect like the one defined before.

1. In scene view select any object animated with Mecanim. It must have Animator component attached and a state machine with two or more states.
2. In the Inspector window click Add Component button and select EffectsController.
3. In the EffectsController inspector select animator, add one binding to the Bindings list, and expand it.
4. In the Binding inspector set State Name property to any of existing states by typing "<Layer Name>.<State Name>" without quotes.
5. Expand the Effects property and add 1 (one) effect and expand it.
6. In the Effect inspector set the Prefab property to previously created sample effect prefab.
7. Done!

Now you can run the scene and see effect appears when object goes into selected state and disappears when state is changing.

## Additional editor features

* By default effects appears as transform children of the object EffectsController is attached to. You can change this behavior by setting the Subject property in the Effect inspector.
* You can use Animator component from any GameObject in the scene.
* Use Reset On Replay flag in the Effect inspector to send Reset message to effect every time it is played.
* 

## Scripting effects

Mecanim Effects utilizes [Unity 3D messaging system](http://docs.unity3d.com/Documentation/ScriptReference/Component.SendMessage.html) to introduce the custom scripting feature. EffectsController can send three types of messages to the game object it is attached to.

* **Enter Message** is sent in Update cycle in the first frame of the selected animator state;
* **Update Message** is sent in Update cycle every frame the selected animator state remains active;
* **Timer Message** is sent in Update cycle when timer reaches the timer treshold and only if timer treshold is more than zero;
* **Exit Message** is sent in Update cycle in the frame following the last frame of the selected animator state.

Messages are sent only if name is explicitly specified in the Inspector window. When appopriate field is blank no message is sent to the object. Messages are sent using [SendMessageOptions.RequireReceiver](http://docs.unity3d.com/Documentation/ScriptReference/SendMessageOptions.RequireReceiver.html) option value, so there will be a warning about unhandled messages.

Example of message handler in C#:

````c#
void OnBaseLayerMoveEnter(EffectUpdateEventArgs e) {
   // your logic here
}
````

EffectUpdateEventArgs class definition: 

````c#
public class EffectUpdateEventArgs : System.EventArgs {
  public EffectsController controller;
  public int layerIndex;
}
````

Using EffectsController instance along with layerIndex value you can query and manipulate the handled animator's state or transition information. EffectUpdateEventArgs instance is sent along with every message. You are not expected to store the reference to the instance between calls or change its fields values.

## License

[WTFPL – Do What the Fuck You Want to Public License](http://www.wtfpl.net/)

See license.txt for full license text.

## Roadmap

* Custom inspector UI - default inspector looks creepy and unintuintive, a new cool Inspector UI is coming soon!
* Cross-state effects caching - states with same effect bound can share cachedInstance.
* Continious effects - effects do not disappear in transition if the next state have similar effect attached. This is possible only with cross-state caching.
