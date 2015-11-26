# Keenou

Keenou is designed to make encrypting your personal and private data as easy as possible, even if you have no experience with encryption.  

Keenou-pGINA is the Credential Provider portion of Keenou based on pGina, which handles setting up your encrypted home folder upon login.  Only local accounts are supported so far. 

## How it works

### Home Folder Encryption

Use Keenou to migrate your home folder data to an encrypted drive with the click of a button.

Keenou will automatically create an encrypted drive (using whirlpool and AES by default with [VeraCrypt](https://github.com/veracrypt/VeraCrypt)) and start copying your current home folder data over to the encrypted folder.  

When finished, it will alert you to log out of your account.  Upon the next login, Keenou-pGINA will finish the migration (moving over files that were locked during the first attempt) and switch your profile over to the encrypted drive automatically.  Note that you should exit out of OneDrive beforehand, since it can cause issues with the migration process. 

Subsequent logins will simply use this encrypted drive. 

### Cloud Storage Encryption

Protect your cloud storage data by encrypting it with Keenou with the click of a button!  Keenou uses the latest version of [encfs4win](https://github.com/jetwhiz/encfs4win), a Windows port of the [EncFS](https://github.com/vgough/encfs) Encrypted Filesystem (using 'paranoid' mode). 

## Caveats

### Home Folder Encryption 
* Currently uses Directory Junctions to link user profile to encrypted folder
  * As a result, files deleted in home folder are put in *unencrypted Recycling Bin* of system drive
  * Do *not* delete sensitive files in your encrypted home folder -- shred them in-place to be safe!
  * Win7+ gets very angry when you move the home directory off of the system drive
* Password of encrypted container **must** match your Windows User Login password
  * If you ever change your Windows password, you must manually change the password for your encrypted container (not yet implemented) 
* Due to current limitations of pGina, only login via username-password combo is possible 
  * If you're using a fingerprint scanner right now, you must revert to your password when your home folder is encrypted
* Google Drive might be unable to locate your Google Drive folder after encryption of your home folder.  You can simply re-select your Google Drive folder upon encryption of your home folder (for the first login after migration) to fix this issue. 
* Microsoft OneDrive will break the migration process, since it will lock a file in your cloud folder even when you are not logged in to Windows.  Please exit out of OneDrive before attempting the migration process.  Subsequent logins do not have this issue. 

### Cloud Storage Encryption
 * It is not yet possible to decrypt your data across multiple computers 
 * It is not possible to share files with others (since they will receive an encrypted file) 
 * Changing your encryption password has not yet been implemented 

### General 
* During the beta testing phase, Keenou will rename your original (unencrypted) data folder to FOLDER.backup (instead of shredding the data in it).  This is for recovery purposes until the software is no longer in beta testing.  This folder is safe to shred manually once you have determined that the migration was successful. 
