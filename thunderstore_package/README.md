# PierceBugFix

Fixes piercing so that piercing shots hit the advertised number of enemies.

Currently there is a hardcoded limit of 5 hits before piercing shots stop checking for additional
hits. However, a single enemy can use up multiple of these hits, drastically reducing the number of
enemies that are actually hit, particularly for wide enemies.

This mod removes this hardcoded limit. It does so by directly modifying the assembly instructions of
the `BulletWeapon.Fire` and `Shotgun.Fire` methods. It changes the `inc` instruction which
increments the number counting up to 5 into a `nop` or do nothing instruction. This causes the
number to never reach 5, completely removing the hardcoded limit. Shots still naturally terminate
when hitting something non-damageable or if their actual piercing limit is reached.

It additionally changes the behaviour of shotguns piercing to no longer re-evaluate the spread on
every pierce. In the base game every pierce causes the spread of the shotgun shot to continually
increase, this mod fixes this so that each pellet simply continues on its original trajectory.

You can find the dev feedback thread on the GTFO modding discord here:
<https://discord.com/channels/782438773690597389/1263680596643680338>

### Known Issues
None! Please let me know if you encounter any!

### Acknowledgements
Kasuromi for [SniperMeleeFix](https://thunderstore.io/c/gtfo/p/Kasuromi/SniperMeleeFix/), which much
of this fix is based on. randomuserhi for suggesting the use of no-ops and for implementing the fix
for shotguns! Dinorush for initial discussion and for implementing the fix for
shotgun spread and new unity update!
