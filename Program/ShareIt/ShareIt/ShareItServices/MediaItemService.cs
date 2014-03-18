using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;

namespace ShareItServices
{
    /// <summary>
    /// This class implements all action that can be performed on media item information.
    /// </summary>
    public class MediaItemService : IMediaItemService
    {
        private IBusinessLogicFactory _factory;

        /// <summary>
        /// Construct a MediaItemInformationService which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public MediaItemService()
        {
            _factory = BusinessLogicFacade.GetTestFactory(); //Remember to change this
        }

        /// <summary>
        /// Construct a MediaItemInformationService object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the MediaItemInformationService should use for its logic.</param>
        public MediaItemService(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get a list of media item information about a given media item.
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A list of MediaItemInformation</returns>
        public MediaItem GetMediaItemInformation(int mediaItemId, string clientToken)
        {
            try
            {
                return _factory.CreateMediaItemLogic().GetMediaItemInformation(mediaItemId, clientToken);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }
    }
}
