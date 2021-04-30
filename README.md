# Sftp-Ftp-Handler
Interface that can handle both SFTP as well as FTP in a single Instance, also keeps collection alife.

## Preparation

Place all .cs Files in your desired Utilities Folder and rename the namespaces of the given Files.

## Code Usage

```
  FtpConfiguration ftpConfiguration = new FtpConfiguration()
  {
      Host = "YourHost",
      Username = "YourUsername",
      Password = "YourPassword",
      // true if it's sftp, false if it's ftp
      IsSFTP = true,
  };

  using (IFtpHandler ftpHandler = FtpHelper.GetInstance(ftpConfiguration))
  {
    ftpHandler.[YourDesiredFtpAction]
  }

```
