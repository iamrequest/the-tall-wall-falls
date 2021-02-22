# The Tall Wall Falls: The Inner Gate's Last Stand

A fast-paced, physics-based, Attack on Titan inspired game. Swing around from the grapple hooks on your swords, and take down huge enemies, before they destroy the gate!
This was my submission to the "[Brackeys Game Jam 2021.1](https://itch.io/jam/brackeys-5)" game jam, which took place over 7 days.

<p align="center">
    <img src="./readmeContents/thumbnail_youtube.png">
</p>

You can download the game for free [here, on itch.io](https://request.itch.io/the-tall-wall-falls).

You're free to do whatever with this code, but if you do use it, it'd be real cool of you to link back to this page or the itch.io page (or both). Thanks!


## Setup

  1. Clone this repo
  2. Purchase and install [Shapes](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167) from the Unity Asset Store (Freya is super lovely, and the asset is really good - you should consider picking it up anyways!)
  3. Install the SteamVR plugin from the Asset Store

## Some topics of interest in this repo

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

## Assets and Third Party Credits

###Project Settings:
  * Unity 2019.4.5f1
  * Universal RP (7.3.1)
  * SteamVR 2.7.2 (sdk 1.14.15)
  * ProBuilder (4.2.3)
  * ProGrids (Preview.6 - 3.0.3)
  * TextMesh Pro (2.0.1)
  * Visual Effect Graph (7.3.1)
  

###Third Part Assets:

<table>
	<tbody><tr>
		<td><strong>Resource</strong></td>
		<td><strong>Author</strong></td>
		<td><strong>License</strong></td>
		<td><strong>Modifications</strong></td>
	</tr>
	<tr>
		<td><a href="https://github.com/FreyaHolmer/Mathfs">Freya Holmer's Math Library (Mathfs)</a></td>
		<td>
<a href="https://twitter.com/FreyaHolmer">Freya Holmer</a><br></td>
		<td><a href="https://github.com/FreyaHolmer/Mathfs/blob/master/LICENSE.txt">MIT License</a></td>
		<td>
<br></td>
	</tr><tr>
		<td><a href="https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167">Shapes</a></td>
		<td><a href="https://twitter.com/FreyaHolmer">Freya Holmer</a></td>
		<td>Paid asset store asset<br></td>
		<td>Not included in git repo<br></td>
	</tr>
	<tr>
		<td><a href="https://gist.github.com/mstevenson/4958837">mstevenson's Configurable Joint Extension methods</a></td>
		<td><a href="https://gist.github.com/mstevenson">Michael Stevenson</a><br></td>
		<td>
<br></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://www.mixamo.com/#/?query=manne&type=Character">Mixamo Mannequin Model</a></td>
		<td>
<br></td>
		<td>
<br></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td>Kenny's Low Poly Weapon Pack (No longer available for download)</td>
		<td><a href="https://www.kenney.nl/assets">Kenny</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>Tweaked longsword model</td>
	</tr>
	<tr>
		<td><a href="https://www.kenney.nl/assets/city-kit-suburban">Kenny’s City Kit (Suburban)</a></td>
		<td><a href="https://www.kenney.nl/assets">Kenny</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://www.kenney.nl/assets/fantasy-town-kit">Kenny’s Fantasy Town Kit</a></td>
		<td><a href="https://www.kenney.nl/assets">Kenny</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://www.kenney.nl/assets/game-icons">Kenny’s Game Icons 1</a></td>
		<td><a href="https://www.kenney.nl/assets">Kenny</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://www.kenney.nl/assets/game-icons-expansion">Kenny’s Game Icons 2</a></td>
		<td><a href="https://www.kenney.nl/assets">Kenny</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>Tweaked an icon to make an HMD icon</td>
	</tr>
	<tr>
		<td><a href="https://www.kenney.nl/assets/ui-pack-space-expansion">Kenny’s UI Pack; Space Expansion</a></td>
		<td><a href="https://www.kenney.nl/assets">Kenny</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://www.kenney.nl/assets/voiceover-pack-fighter">Kenny’s Voiceover Pack: Fighter</a></td>
		<td><a href="https://www.kenney.nl/assets">Kenny</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://www.kenney.nl/assets/impact-sounds">Kenny’s Impact Sounds</a></td>
		<td><a href="https://www.kenney.nl/assets">Kenny</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://www.kenney.nl/assets/interface-sounds">Kenny’s Interface Sounds</a></td>
		<td><a href="https://www.kenney.nl/assets">Kenny</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://opengameart.org/content/rpg-sound-pack">RPG Sound Pack</a></td>
		<td><a href="https://opengameart.org/users/artisticdude">artisticdude</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://opengameart.org/content/grass-001">Grass 001</a></td>
		<td><a href="https://opengameart.org/users/lamoot">Lamoot</a></td>
		<td><a href="https://creativecommons.org/licenses/by/3.0/">CC-BY 3.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://opengameart.org/content/dirt-001">Dirt 001</a></td>
		<td><a href="https://opengameart.org/users/lamoot">Lamoot</a></td>
		<td><a href="https://creativecommons.org/licenses/by/3.0/">CC-BY 3.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://opengameart.org/content/outdoor-stone-floor">Outdoor Stone Floor</a></td>
		<td><a href="https://opengameart.org/users/sindwiller">Sindwiller</a></td>
		<td><a href="https://creativecommons.org/licenses/by-sa/3.0/">CC-BY-SA 3.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://opengameart.org/content/heroes-theme">Heroes Theme</a></td>
		<td><a href="https://opengameart.org/users/alexandr-zhelanov">Alexander Zhelanov</a></td>
		<td><a href="https://creativecommons.org/licenses/by/3.0/">CC-BY 3.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://opengameart.org/content/5-chiptunes-action">5 Chiptunes (Action)</a></td>
		<td><a href="https://opengameart.org/users/subspaceaudio">SubspaceAudio</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
Converted from .wav to .ogg<br></td>
	</tr>
	<tr>
		<td><a href="https://opengameart.org/content/menu-music">Menu Music</a></td>
		<td><a href="https://opengameart.org/users/mrpoly">mrpoly</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://opengameart.org/content/clouds-skybox-1">Clouds Skybox 1</a></td>
		<td><a href="https://opengameart.org/users/lukerustltd">Luke.RUSTLTD</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://opengameart.org/content/zombie-skeleton-monster-voice-effects">Zombie / Skeleton / Monster Voice Effects</a></td>
		<td><a href="https://opengameart.org/users/arcadeparty">ArcadeParty</a></td>
		<td><a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://opengameart.org/content/fleshy-fight-sounds">Fleshy Fight Sounds</a></td>
		<td><a href="https://opengameart.org/users/willleamon">will_leamon</a></td>
		<td><a href="https://opengameart.org/content/oga-by-30-faq">OGA-BY 3.0</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://www.fontsquirrel.com/fonts/chomsky">Chomsky Font</a></td>
		<td><a href="https://www.fontsquirrel.com/fonts/list/foundry/fredrick-brennan">Fredrick Brennann</a></td>
		<td><a href="https://www.fontsquirrel.com/license/chomsky">SIL Open Font License</a></td>
		<td>
<br></td>
	</tr>
	<tr>
		<td><a href="https://www.fontsquirrel.com/fonts/dejavu-serif">DejaVU Serif Font</a></td>
		<td><a href="https://www.fontsquirrel.com/fonts/list/foundry/dejavu-fonts">DejaVu Fonts</a></td>
		<td><a href="https://www.fontsquirrel.com/license/dejavu-serif">DejaVU Fonts License</a></td>
		<td>
<br></td>
	</tr>
</tbody></table>
