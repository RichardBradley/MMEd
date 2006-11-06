using System;
using System.Collections.Generic;
using System.Text;

// Holds global (i.e. not instance specific) settings
// like the meanings of various constants

namespace MMEd
{
  public class GlobalSettings
  {
    public PowerUpType[] PowerUps;

  }

  public class PowerUpType
  {
    public string Name;
    //TODO: weapon icons here?
  }
}
