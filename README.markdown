MMEd - Micro Machines v3 level editor
=====================================

MMEd is a level editor and asset explorer for Micro Machines v3, a PlayStation 1 game released in 1997.

There is [an overview and some screenshots of the editor on my website](http://bradders.org/MMs/).

![MMEd screenshot](http://bradders.org/MMs/scrn/truckers-luck.600.png)

How to use the editor
=====================

Quick start for the impatient
-----------------------------

Here's how to change the ommer from Cheesey Jumps into mines. Hopefully this will serve as an illustrative example.

Preparatory steps:

* Check out the source code
* Get a copy of the "PSX" emulator from [http://www.emulator-zone.com/doc.php/psx/psx_em.html](http://www.emulator-zone.com/doc.php/psx/psx_em.html)
* Get a copy of the files from the CD onto your PC
* Run MMEd (needs .NET 2.0); optionally you can do this from within Visual Studio 2005 - this is a bit slower but helps if it crashes!

Editing your course:

* Click File > New (CTRL+N). Browse for your copy of the CD image, and select "BREAKY1 - CHEESEY JUMPS" from the course dropdown.
* Expand "MMv3 Level" and then the "SHET" chunk in the tree view, and select "201 bkftable" (which is the "Flat" with the ommer on it).
* Select the "Flat" viewer pane. It will show you details of the selected chunk, including a list of weapons.
* Change "Ommer" to "Mines" in the dropdown in the middle of the page.
* Click "Commit" to save your changes to the file (in memory).
* Click File > Save (CTRL+S). Save your file somewhere - make sure you save as a MMEd Save File (.mmv file) for best results.
* Observe that a new entry appears in the tree "Version <Current Date/Time>" - click here to see a summary of what you changed, and you can revert back to this version in future if necessary.
* Click File > Publish (CTRL+P). Browse for a suitable location to save your level in binary format. Check the "Update CD Image" box and browse for your copy of the CD image, make sure the correct course is selected in the dropdown, and rename the course if you wish. Click Publish.

Testing your changes:

* Run the PS emulator using the modified CD image (select the BIN, not the CUE)
* Navigate to Cheesy Jumps in multiplayer mode - just before clicking Ok on the course
* Click Quick Save > Quick Save 1 (F6).
* Play.
* Notice that the weapon by the first corner which was an ommer is now mines.
* ...
* Profit!

Next time, as soon as you've clicked Publish in MMEd just click Quick Save > Quick Load 1 (F1) in the emulator and you get back here ready to play your new course. Provided you quick save somewhere before you've clicked Ok to select your course, you don't need to restart the emulator to pick up changes to your course.

Some notes:

* Although the MMEd Save File format is great because it gives you a version history, it's always possible something will go horribly wrong. For this reason, make sure you keep a few backups of the raw MMs level binary files - these are the ones produced by the Publish option (and conveniently a few automatic backups are kept in a "Backup" folder, as per the option on the Publish screen).
* Changes to course names don't show up if you just use Quick Load - you need to reboot your virtual PS1 to get them to show up.

Other things you might want:

* Use the "XML" viewer pane to do your editing by hand

Burning CDs for the PS
----------------------

Update your CD image, using the same process as above. If this doesn't work, or you're old-fashioned:

* Open CDMage from [http://www.softpedia.com/get/CD-DVD-Tools/CD-DVD-Images-Utils/CD-Mage.shtml](http://www.softpedia.com/get/CD-DVD-Tools/CD-DVD-Images-Utils/CD-Mage.shtml)
* Open the CUE file from your working copy of the CD. Open the BIN file when it prompts you 
* Navigate to your course (e.g. BREAKY1.DAT) in the CD file tree, right click on it, and choose "import"
* Find your published course binary (e.g. BREAKY1-NEW.DAT), and import it

Having got an up-to-date CD image, burn it to CD:

* Get or make BIN/CUE files for the cd which you'd like to burn
* Open "fireburner"
* Choose `file -> import tracks from cuesheet`, and open the CUE file. Click ok on the dialog. (NB - if you have the CUE/BIN files open in CDMage, this seems to fail.)
* Insert a normal CD-R 
* Choose `create CD -> burn`
* wait...
* Test it in the PS.
* You can also use imgburn which is apparently better than fireburner. When it verifies after burning it may complain that some of the language-specific files don't match, but the disc seems to work fine anyway so ignore these warnings.

Adding stuff
-----

Adding stuff to a level is reasonably easy. However you need to bear in mind a couple of important points:

* We don't yet know how to change the size of the level files on the CD. It's therefore important that the file size stays unchanged. Fortunately the level files have a load of zero bytes padding out the end of the file, and we can play around with this space. On the whole MMEd will manage these trailing bytes for you (and will warn you if you exceed them - you'll need to free up space before you can save the level). But keep this limitation in mind. And if you do need to fiddle with the size manually (e.g. if you edit the XML representation of the level directly) here's how:
 * Save your level in .dat format and note the file size (you'll need to go into Properties to see it in bytes rather than kB)
 * Make your changes, then save again (again, you'll need a .dat file so it'll let you save despite the file size being wrong)
 * Calculate the number of bytes by which the file size has changed
 * Go to the SHET node and select the XML viewer. Find the TrailingZeroByteCount at the very end
 * Reduce this by the number of bytes your file size has increased
 * Commit, save, and check that the file size has gone back to its original value
 * Proceed as normal
 * Or you can ignore all this, stick to .dat format throughout, and use CDMage to update your CD image. This will automatically truncate / add zeros to your level to fit (so it's safe provided you haven't actually used more bytes than you have). But if you do this you'll lose many of the advantages of MMEd such as automatically updating the CD image in a couple of clicks, so you wouldn't want to do that.
* There are some limitations we don't understand. For example:
 * Try adding 100 pool balls (or Softwire Oranges, as they appear in fact to be) to Rack 'N Roll. You may have room in the file, but the level won't load. Presumably this is because the balls are too complicated (the 3D representation has a lot of faces). If you're feeling scientifically inclined, you could try to work out what the limitation actually is.
 * You can't have more than 35 flats in a level (probably - more than that crashes Rack, and no standard course has more than 35).

If you run out of space in your level, there are a few things you can do about it:

* Use the "Optimise Level" action on the Actions viewer. This will try to get rid of unnecessary stuff from your level file - specifically Bumps, Odds and Camera Positions (although the latter is only really feasible if you're happy with a single camera position throughout multi-player mode, for reasons that you might be able to deduce if you read more about camera angles below). To save space, you need to use the "Compact" option - "Reindex" will just reduce the number in use without actually freeing up the space in the file. Since MMEd will automatically create new Bumps and Odds whenever necessary (when you're editing Flats in Grid view) there's generally no harm in doing this, and it can free up quite a bit of space.

* Adjust the "scale" of some Flats. The best example is the floor on the pool table courses - this is a fairly big (e.g. 14x14) flat, with scale (100, 100). That means each of those 14*14 tiles is size 100x100. You don't actually drive on the floor, so this is just for display purposes - you can save 195 tiles of space in your level file by changing the floor sheet to be 1x1 in size, and 14000x14000 scale. That's 195 tiles you can use on a more interesting Flat, such as a new pool table or a Big Jump (TM). You can actually take this a step further and remove unused bits of Flats altogether. This might not be a good idea for the floor (doing so prevents you from jumping over the floor, on Rack), but the pool tables are made up of several layers of flats and you can generally only drive on bits of each layer. Split up the layers into multiple flats covering only the bits you need, and you get yet more space.
* Delete some stuff! If you want to add another object, delete one that's not really doing much. In theory you might even be able to delete whole object definitions from the OBJT chunk (provided they're not referenced, and you renumber appropriately) - although I don't think anyone's yet dared to try...

Using the 3D Editor
----

The 3D editor currently allows you to move objects around (but not rotate them) and save the results.
Controls

* Move camera: Right mouse button + move mouse
* Rotate camera: Left mouse button + move mouse
* Zoom camera: Mouse wheel (This is really moving the camera along its z-axis)
* Select object: CTRL + left mouse (if it selects sucessfully then you should see a red box around the object)
* Move object: ALT + left mouse + move mouse (in the 3 ortho-views only)
* Move object 2: Keyboard arrow keys (will move the object relative to the last view clicked on with the mouse). Hold down CTRL for fine control.
* Rotate object: SHIFT + left mouse + move mouse (in the 3 ortho-views only) will rotate around the camera's view axis
* Rotate object 2: SHIFT + arrow keys (will rotate object relative to last view click on with the mouse). Hold down CTRL for fine control.

WARNING: It does occasionally crash, so save often!

WARNING 2: Rotating is a bit bugged... you can safely rotate in the top down view, and up to 90 degrees in the other two. But after that, things start to go a bit wrong!

Editor Development
----

### Bugs
Known bugs in the editor:

* Sometimes the key waypoints get themselves in a pickle, perhaps after a bout of excessive waypoint editing. One symptom is that in the editor certain squares get into a state where it is awkward (if not impossible) to add or remove a waypoint or key waypoint. Another is that the level refuses to load when you try and play it (although there are many other causes of loading failure). To resolve it, inspect the key waypoint xml and remove duplicate or overlapping entries by hand.
* The optimise tool doesn't really work for camera positions, because it doesn't understand the difference between single and multi-player. Hence it corrupts your camera positions. It shouldn't. MWR

### Feature requests
Things people would like:

* Some sort of copy-and-paste from one flat to another, or one version of a course to another (although as DMS points out there's a certain amount of this you can do with copy & paste in the XML. If you're using a .mmv file it'll even offer to correct the file length when you save after hacking with the XML).
* Grid view editor:
 * Allow objects to be moved around.
 * Allow changing the size of the "paintbrush" when editing.
 * Easier access to edit items that are visible on the grid, e.g. double-click to edit bump square (or right-click and get a menu of all the things you can edit) - editor opens in new window, perhaps.
 * Tidy up the potential conflict between the Respawn, Steering and Behaviour edit modes.
* Flat editor:
 * Allow naming of objects, to make it easier to see what's in the dropdowns. (Could even allow grouping of objects, for bulk move operations etc).
 * Make the flat view display faster when you've got a lot of objects & weapons.
* 3D editor:
 * Needs to be able to rotate objects reliably (need to perfect how to map "normal" rotations on to MMs rotation vectors)
 * Needs to crash less often
 * Needs code tidy up / commenting
 * Highlight currently active view (so that you can tell what effect the arrow keys will have)
 * Allow more than one object to be selected at once
* 3D viewer / editor:
 * Needs to unload some textures from video memory when they're no longer used. It currently keeps on adding new textures each time you switch to a new level, or switch meta data type in view meta data mode. I don't know what happens if you take this too far: it'll probably crash. I've certainly found that I get OutOfMemory exceptions if I leave the app running in 3D editor for a few hours
* Image editors (need to be careful with PS paletting / VRAM restrictions). Arguably export as BMP, which is already possible, is sufficient.
* Object export/import: save an object and associated texture data from one level as a stand-alone file which can then be imported into another level, ideally with the editor finding a spare bit of VRam and sorting the texture stuff out automatically. (It'll be tricky to sort out the VRAM considerations automatically, but shouldn't be impossible)
* Wizards for complicated operations like adding a flat that lets you drive up an object (playing card, cheese, suspension bridge etc)
* Repair tool, that looks for problems like corrupt waypoints and anything else that you or MMEd might have corrupted.

* General nice to haves:
 * Better C# proxy objects
 * Find a nice C# pretty printer (like Jalopy for Java). I haven't bothered to indent code in a few places, pending this TODO
 * Any functionality to make the position, dimension and scaling measurements easier to understand and modify would be useful. At the moment it's tricky to understand how all the numbers relate to each other, and requires a bit of trial and error.

### Knowledge Base

#### Tex Meta Data

Most flats that you can drive along (specifically, those with the HasMetaData flag set) have an array of 8 data values for every square on the flat - TexMetaData. These values have the following meaning (name in eTexMetaDataEntries is in brackets):

* Zero (Steering). Links to the "steering" images. These provide steering direction instructions to AI players (which don't follow them literally, but are generally guided in that direction), and also indicate which directions cars should face when they respawn at this location. The odd divides the grid square down into 8x8 sub-squares. Each of these specifies the steering direction as follows:
* If the "Four" value is 0, then steering value x corresponds to steering x * 22.5 degrees clockwise from N.
If the "Four" value is y where 1 <= y <= 4, then steering value x corresponds to steering (y * 90) - (x * 22.5).
* One (Unknown). Unknown. Zero everywhere except School2-5, which have a few 2, 3 and 117 values on the floor near the edges.
* Two (Behaviour). Square-wide meta-data which controls whether you tumble when you fall off things, whether you can respawn in a square, what speed the AI cars go at, and almost certainly some other stuff still to be discovered. In the range 0-16, except for a couple of 27 and 192 values on School1-5. FlatChunk.Behaviours has a list of the 0-16 values and their meanings (eBehaviourTypes).
* Three (CameraPos). Camera position - ID lookup into the camera position data (see "Camera pos" below).
* Four (Orientation). Basic orientation of the square: 0/4 is N, 1 is E, 2 is S, 3 is W. This affects the direction in which the Zero (Odds / Steering) data is applied. Note that there are a few random values outside the 0-4 range on School1-5 (one begins to wonder whether the designer of the school courses was on something...)
* Five (Waypoint). 1 is the start of the course, and you count up from there.
* Six (Bumpmap). ID lookup into the bump data, which gives you 8x8 sub-squares that define behaviour like sticky goo, fiery chemicals, teleports, etc. The C# code has a list of the known ones in BumpImageChunk.cs.
* Seven (RespawnPos). Respawn position - if you read the byte as two hex values 0-15 instead of one value 0-255, then the two values are the co-ordinates of the respawn location relative to the NW corner of the square. The range of the co-ordinates covers four squares: the square itself plus its neighbours to E, S and SE. So e.g. 0 is top-left; 8 is top-left of the square to the S; 52 is roughly the middle of the square; 195 is roughly the middle of the square to the E. On the whole respawn positions outside the square aren't used, but there are some examples (and not just on the school courses either...). This could presumably be a way of preventing respawns in a square, although the Two values are much more commonly used.

#### Camera positions

The camera pos numbers refer to a set of camera positions defined per level - look in the SHET XML near the bottom (immediately before the long BumpImages section). Each position is defined by three numbers: angle, distance and elevation (listed in that order).

Angle uses the same range as the z rotation value used for object positioning: 0 is looking "north" directly up the y axis, 1024 is looking "east" (i.e. toward increasing x values), and +/-2048 and -1024 are "south" and "west".

Distance is linear distance along the xy plain and elevation is height about the xy plain. They use the same range (probably the same as points in world co-ordinates, but I haven't checked that), so 0 is where the car is, 500 is not particularly far away, 1000 is a bit further away than is normally used in the game. 2500 elevation is enough to give you a birds-eye view of most of one of the Rack pool tables.

All the camera positions up to 50 are ignored in multiplayer mode (and one default position is used for all of them). Values over 50 are used in the normal way for multiplayer games, and the single player camera position is the one numbered 50 less; e.g. if the value is set to 53 then multiplayer uses camera 53 and single player uses 3.

#### Tumbling

If you fall off a sufficiently high cliff (or just jump over something such that you're a long way above the ground), you "tumble". This behaviour can be altered as follows:

* The lowSurvivalHeight bump texture makes you tumble even if you don't fall very far.
* The Behaviour values 3 and 13 allow you to fall a long way without tumbling.

#### Teleports

Certain bump textures cause you to teleport to new locations, occasionally with various fancy movements around the screen (think bunsen burners). The behaviour of the teleports, including exit locations in world coordinates, appears to be hard-coded. Details of some teleports are documented in BumpImageChunk.MakeBumpTypes.

#### Still to be discovered

* Animations (e.g. toads, bumble-bees, microscopes, teleporters). Seems likely to be in the "dunno" chunk after OBJT and before SHET
* Background music. This isn't in the level data; importing a different level on top of Hair of the Dog doesn't get rid of the funky music.
* How to change car type & speed for a level. This appears to be in the main executable (since i) it displays the car type during level selection, before loading any level data, and ii) if you import one level file in place of another you get the original cars on the new level). There seems to be some variation in the handling of the cars within the level file however (for example, if you import Swerve Shot over Cereal Killer, you get slow race cars which are nonetheless faster than the normal Swerve Shot ones). Perhaps the executable defines the car type to use and the level data defines the vehicle power level. (Note that in single player mode you can win cars to play in one of the party modes; with this, if you win a car you've already got, it gets upgraded to level 2 (or something like that). So perhaps the executable defines the car type to use and the level data defines the vehicle power level in multi-player).
* The last remaining unknown bump textures.
* Objects have an unknown flag and an unknown short field. The short field seems to have some sort of z-order property, in that objects with smaller numbers appear over the top of those without. But it's more than this, because it affects the height of the object too - in an experiment on Rack, increasing ShortUnknown had a similar effect to increasing Z by about half as much. The flag has no visible impact on the position of or the ability to jump through or drive under certain objects (specifically, playing cards on Rack).
* Misc. fields and flags for the shets, objects, etc. Flag C on Flat seems to be whether it's solid.

Playstation Modding and How to play CDRs on a Playstation
----

We can play CDRs on a PS2 slimline by:

* Obscuring the PS2's open CD tray detector as per [http://www.instructables.com/id/Mod-a-PlayStation-2-Slimline-for-FREE!-NO-CHIPS!/?ALLSTEPS](http://www.instructables.com/id/Mod-a-PlayStation-2-Slimline-for-FREE!-NO-CHIPS!/?ALLSTEPS)
* Turn on the PS2 with a real copy of MMv3 in the tray
* Wait for it to boot and reach a stable screen where the disc is not being accessed (e.g. the choose language screen, or the choose players screen)
* Swap in the modified MMv3 CDR quickly and carefully
 * You will need to momentarily stop the CD spindle with your finger. Do this for as little time as possible, as it damages the motor and the CD drive will burn out fairly quickly.
* Continue the game. The PS will read from the CDR instead of the original disc and be none the wiser.

But how does this actually work? Here's the long version:

* The PS2 anti-copying works by:
 * Valid PS2 discs have a "magic" checksum in one of their sectors which is invalid according to the CD standard (i.e. to most CD players it looks like disc damage)
 * No CD burners will allow you to burn a CD which has this magic checksum, because they won't let you burn pre-damaged discs
 * The magic checksum is verified by the PS hardware when the disc is first booted
 * The PS will not allow you to swap discs after it has booted (if it notices you opening the tray, it freezes and forces you to restart)
* So to play a modified game if you have a real copy, you just need to do the physical mod described in the instructables link above
 * We will boot from a real copy of MMv3
 * This real copy has the magic checksum sector, so the PS boots happily
 * At a suitable point when the game is not accessing the disc (e.g. at the choose language screen, or at the choose players screen), we'll swap in the CDR of MMv3
 * The PS will not notice that the tray was open
 * The next time the PS reads from disc (e.g. to load a level), it will happily read from our CDR
* There is a related mod system called "swapmagic" which allows you to play CDR copies of any PS game (without needing a real copy)
 * They have written their own "PS game" which has a magic checksum sector (using expensive non-consumer branded CD presses instead of CD burners)
 * The swapmagic game doesn't do anything except pass the PS boot loader and pause
 * The swapmagic game asks you to swap in your PS game CDR and press "OK"
 * You must have physically modded your PS to disable the "open tray" detector
  * Either by following the instructables link above
  * Or they recommend the "magic switch pro" or "slide tool"
  * (Note that they claim on their site "you don't need to mod your PS2". By this they must mean that the simple physical mods which disable the open tray detector do not "count" as real "mods", like the old systems where you had to carefully solder in a mod chip.)
 * You then swap in your CDR without the PS's hardware detection from 1.d being triggered, and press "OK"
 * The swapmagic game then does a "soft reset" of the PS memory to boot from the CDR (without re-triggering the checksum detection) and you can then play any PS game from a CDR
