using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
using ShareIt;

namespace ShareIt
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
            catch (ArgumentException ae)
            {
                var fault = new ArgumentFault {Message = ae.Message};
                throw new FaultException<ArgumentFault>(fault);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Finds a specific range of media items.
        /// 
        /// E.g. FindMediaItemRange(1, 3, "token") finds the first 3 media items
        /// per media item type.
        /// </summary>
        /// <param name="from">Where the range must begin. Must be > 0</param>
        /// <param name="to">Where the range must end. Must be > 0</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        /// <exception cref="FaultException&lt;Argument&gt;">Thrown when from or to is &lt; 1</exception>
        /// <exception cref="FaultException">Thrown when the MediaItemType is not recognized or when something unexpected happens</exception>
        public Dictionary<MediaItemType, List<MediaItem>> FindMediaItemRange(int from, int to, string clientToken)
        {
            return FindMediaItemRange(from, to, null, null, clientToken);
        }

        /// <summary>
        /// Finds a specific range of media items of a specific media type.
        /// 
        /// E.g. FindMediaItemRange(1, 3, MediaItemType.Book, "token") finds the first 3 books.
        /// </summary>
        /// <param name="from">Where the range must begin. Must be > 0</param>
        /// <param name="to">Where the range must end. Must be > 0</param>
        /// <param name="mediaType">The type of the media item. E.g. MediaItemType.Book for books</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        /// <exception cref="FaultException&lt;Argument&gt;">Thrown when from or to is &lt; 1</exception>
        /// <exception cref="FaultException">Thrown when the MediaItemType is not recognized or when something unexpected happens</exception>
        public Dictionary<MediaItemType, List<MediaItem>> FindMediaItemRange(int from, int to, MediaItemType mediaType, string clientToken)
        {
            return FindMediaItemRange(from, to, mediaType, null, clientToken);
        }

        /// <summary>
        /// Finds a specific range of media items matching the search keyword.
        /// 
        /// E.g. FindMediaItemRange(1, 3, "money", "token") finds the first 3 media items
        /// per media item type with some information that contains "money".
        /// </summary>
        /// <param name="from">Where the range must begin. Must be > 0</param>
        /// <param name="to">Where the range must end. Must be > 0</param>
        /// <param name="searchKey">The search keyword. This will be matched against all information about all media items</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        /// <exception cref="FaultException&lt;Argument&gt;">Thrown when from or to is &lt; 1</exception>
        /// <exception cref="FaultException">Thrown when the MediaItemType is not recognized or when something unexpected happens</exception>
        public Dictionary<MediaItemType, List<MediaItem>> FindMediaItemRange(int from, int to, string searchKey, string clientToken)
        {
            return FindMediaItemRange(from, to, null, searchKey, clientToken);
        }

        /// <summary>
        /// Finds a specific range of media items of a specific media type matching the search keyword.
        /// 
        /// E.g. FindMediaItemRange(1, 3, MediaItemType.Book, "money", "token") finds the first 3 books
        /// with some information that contains "money".
        /// </summary>
        /// <param name="from">Where the range must begin. Must be > 0</param>
        /// <param name="to">Where the range must end. Must be > 0</param>
        /// <param name="mediaType">The type of the media item. E.g. MediaItemType.Book for books</param>
        /// <param name="searchKey">The search keyword. This will be matched against all information about all media items</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        /// <exception cref="FaultException&lt;Argument&gt;">Thrown when from or to is &lt; 1</exception>
        /// <exception cref="FaultException">Thrown when the MediaItemType is not recognized or when something unexpected happens</exception>
        public Dictionary<MediaItemType, List<MediaItem>> FindMediaItemRange(int from, int to, MediaItemType? mediaType, string searchKey, string clientToken)
        {
            try
            {
                return _factory.CreateMediaItemLogic().FindMediaItemRange(from, to, mediaType, searchKey, clientToken);
            }
            catch (ArgumentException ae)
            {
                var fault = new ArgumentFault { Message = ae.Message };
                throw new FaultException<ArgumentFault>(fault);
            }
            catch (InvalidOperationException ioe)
            {
                throw new FaultException(new FaultReason("Error when casting the MediaItemType"));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }
    }
}
