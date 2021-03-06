////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Samples                                          //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: __Entry.cs                                   //
//                                                            //
//      Version: 0.7                                          //
//                                                            //
//         Date: 11/09/2010                                   //
//                                                            //
//       Author: Tom Shane                                    //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//  Copyright (c) by Tom Shane                                //
//                                                            //
////////////////////////////////////////////////////////////////

#region //// Using /////////////

////////////////////////////////////////////////////////////////////////////
using System; 
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Samples
{
  static class __Entry
  {

	  #region //// Methods ///////////
			
		////////////////////////////////////////////////////////////////////////////
		static void Main(string[] args)
    {
      using (AddonControls sample = new AddonControls())
      {
        sample.Run();
      }
    }
  	////////////////////////////////////////////////////////////////////////////
	 
	  #endregion  
	  
  }
}

