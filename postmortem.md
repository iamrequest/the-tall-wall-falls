# The Tall Wall Falls: The Inner Gate's Last Stand - Postmortem

## Some topics of interest in this project

This project was an experiment in a VR physics-based character controller. 
Nausia prevention was the first thing I wanted to target, which is why I added the vignette and speed lines early (with settings to adjust the intensity). 


### Rigidbody-based character controller

This one was a bit challenging since I'm used to Character Controller rigs. I wanted the player to have snappy movement on the ground (achieved via changing the physics material on the player Rigidbody if the player is moving), and lots of control in the air (achieved by an [acceleration ForceMode](https://docs.unity3d.com/ScriptReference/ForceMode.html)). 
The latter probably should've been changed to ForceMode.Force instead of ForceMode.Acceleration, but I found it was much easier to target the enemies from midair with the extra control.

### Cool Jumping

A Rigidbody-based character controller also made jumping more interesting. Since I was already counting my collisions to detect if I was on the ground, it was easy to sum up the normals of my ground collisions so that the player jumps parallel to the ground normal (see also: [CatlikeCoding - Physics](https://catlikecoding.com/unity/tutorials/movement/physics/)).
This change also made wall jumps super easy, since they worked pretty much the same way.

As part of this character controller, I also implemented [Coyote Time](https://developer.amazon.com/blogs/appstore/post/9d2094ed-53cb-4a3a-a5cf-c7f34bca6cd3/coding-imprecise-controls-to-make-them-feel-more-precise), by caching my ground/wall normals for a few frames, or until the player touches ground/walls next.

### Grapple Hooks

Whereas the grapple stuff in my previous project ([Blasty Boy](https://github.com/iamrequest/blastyboy/)) was done via a simple CharacterController lerp position, I instantiate SpringJoints whenever the grapple hook collides with a Collider that has a Rigidbody. These spring joints have a high spring value, to make the ropes more rigid at distance, which came with the downside of springy behaviour.
To achieve rope-like physics, I adjusted the spring joint's maxDistance. As long as the distance between the player and the joint's remote anchor is less than the maxDistance, no joint force is applied. To pull the player towards the remote anchor, I simply reduced springJoint.maxDistance over time towards 0.

In order to prevent physics jankyness, I connect the ConfigurableJoint from the target rigidbody to the player's main rigidbody. Then, I set the local anchor position to match that of the player's true hand. This way, any freak-outs of the physics hand do not end up affecting the player's rope.

### Physics Hands, whose transforms are primarially driven by Configurable Joints.

In lieu of hand models, I decided to just have sword models for player hands. These gameobjects have rigidbodies, and are bound to the player's main rigidbody (The one on the [CameraRig] gameobject) via a pair of ConfigurableJoints (see also: [WireWhiz - Half Life Alyx Hands](https://wirewhiz.com/making-half-life-alyx-physics-hands-in-unity/)).
I wasn't able to tweak the configurable joint's settings to the point where they would stay steady with no collisions, so I had to stabilize the sword's rotations via a [simple lerp script](/Assets/Scripts/VRPlayer/PhysicsHand.cs) when the swords weren't touching anything. More details later on.

### Using Shapes to render the grapple hook ropes

I could've used a LineRenderer to render the grapple ropes instead of using [Shapes](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167), but I wanted an opportunity to try it out!

The basic concept of rendering the ropes comes in 3 steps:

  1. Define a bezier spline consisting of 4 points (start/end points, and 2 control points). The start and end points are bound to the start of the rope renderer, and to the grapple hook projectile.
  2. I manually define forward/backwards, and left/right offsets for each of the 2 control points. Over the lifetime of the projectile's flight, I lerp towards/from those values.
  3. Finally, once I have my spline defined, I use Shapes.PolyLine to approximate the spline shape. I pick ~32 points along the spline, and place the PolyLine's points at those spline points.
  
I also adjust the color of the PolyLine from white to red, where a red color means that the rope is at max distance. This is done via a simple lerp, comparing the player's distance from the projectile position to the spring joint's max distance.

### Interactable Settings Menu

In this project, I added a simple settings menu that featured touch-able buttons, grabbable sliders, and a tab-based page system. This was mostly for adjusting comfort settings, but I ended up using it to drive the game state and difficulty as well.
The end product could use some polish, but it worked out just fine for what I needed.

### ScriptableObject Event Channels

This was a trick I picked up from a [Unity Devlog](https://youtu.be/WLDgtRNK2VE?t=255). The idea is to define UnityEvents in your ScriptableObjects, and reference the ScriptableObject in your scripts. 
This way, the scriptable objects act as a delegate, and will help decouple your classes. For example, my VignetteManager class didn't need any direct reference to my SettingsManager class. Whenever the player enabled/disabled the vignette settings menu option, an event would be raised in the scriptable object. Both classes referenced the scriptable object instead.
This made testing individual components super easy later on.

### Bigraph-based enemy navigation

I needed a way for the enemies to spawn at one end of the field, and navigate to the gate, in such a way that they would get close enough to the buildings to allow for interesting player-interactivity.
The solution I came up with was to create a [bigraph](https://en.wikipedia.org/wiki/Bigraph), that enemies could depth-first search to find a random path to the gate. 
To make their pathing between nodes more interesting, I defined bezier splines at each node that would bridge to the next node. Once the enemy had decided on a path, I stitched each sub-spline into one large spline, which the enemy would use for navigation.

Each enemy would walk the entirety of the stitched spline over the course of X seconds (configurable in the settings menu), plus/minus a random percetange offset.

## Next Steps 

### Physics Hands Config Joint Settings / Rotation Lerp Smothing

This turned out ok - the swords would still freak out a bit when they were in contact with some other rigidbody. If I had more time, I would consider tweaking the settings more, and maybe just lerp the physics hand transform towards the true hand at all times.

### Enemy spline walking

The main issue with spline walking is that the amount of time it takes to walk each sub-spline (ie: from node A to node B) is the same. 
Consider the scenario with 3 nodes (A, B, C), where A->B is 9 meters apart, and B->C is 1 meter apart: Due to how my BezierSpline implementation is calculated, BezierSpline.GetPoint(0.5f) would return the position of B. Pretty sure that's the issue anyways, it was late when I was debugging that part.
The simple fix to this, would be to add more intermediate nodes, all at approximately the same distance from one-another.

The second issue with my spline stitching, is that my stitched spline did not have a continuous velocity. This was due to my setting my control points to Free mode, rather than Mirrored or Aligned. When I tried altering the control point nodes, the sub-spline paths would get pulled around to the point where they would collide with world geometry. Not a problem, since everything is kinematic, but it looked pretty bad.
To fix this, I would again, add more intermediate nodes. That would probably fix the issue.

I would also spend more time creating a custom inspector to add+manage nodes. Creating a circular graph, or orphan nodes, was all too easy when the graph was created by hand.

### VFX and Shaders

I've been working with the Visual Effect Graph, and the Shader Graph lately. If I had more time, I would've added simple VFX for the grapple hook and the sword. Adding an outline shader to the enemies to make them stand out would've been cool too, but getting a depth texture based outline shader probably isn't possible via VR, since each eye renders a separate depth texture. Maybe possible if I add a second camera on the player's head, and I use that depth texture instead somehow?
