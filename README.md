# Keenou

Keenou is designed to make encrypting your personal and private data as easy as possible, even if you have no experience with encryption.  

Keenou-pGINA is the Credential Provider portion of Keenou based on pGina, which handles setting up your encrypted home folder upon login.  Only local accounts are supported so far. 

## How it works

Use Keenou to migrate your home folder data to an encrypted drive with the click of a button.

Keenou will automatically create an encrypted drive (using whirlpool and AES by default) and start copying your current home folder data over to the encrypted folder.  

When finished, it will alert you to log out of your account.  Upon the next login, Keenou-pGINA will finish the migration (moving over files that were locked during the first attempt) and switch your profile over to the encrypted drive automatically.  

Subsequent logins will simply use this encrypted drive. 

## Caveats

* Currently uses Directory Junctions to link user profile to encrypted folder
  * As a result, files deleted in home folder are put in *unencrypted Recycling Bin* of system drive
  * Do *not* delete sensitive files in your encrypted home folder -- shred them in-place to be safe!
  * Win7+ gets very angry when you move the home directory off of the system drive
* Password of encrypted container **must** match your Windows User Login password
  * If you ever change your Windows password, you must manually change the password for your encrypted container (due to limitation of VeraCrypt's command line interface)
* Due to current limitations of pGina, only login via username-password combo is possible 
  * If you're using a fingerprint scanner right now, you must revert to your password when your home folder is encrypted
* During the beta testing phase, Keenou will rename your old user profile to USERNAME.backup (instead of shredding the data in it).  This is for recovery purposes until the software is no longer in beta testing.  This folder is safe to shred manually once you have determined that the migration was successful. 
