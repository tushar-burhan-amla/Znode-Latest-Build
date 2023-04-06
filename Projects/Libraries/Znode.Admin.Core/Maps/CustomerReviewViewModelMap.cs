using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Maps
{
    public class CustomerReviewViewModelMap
    {
        //Bind list of Review Status.
        public static List<SelectListItem> GetReviewStatus(string status)
        {
            List<SelectListItem> statusList = new List<SelectListItem>();

            statusList = (from item in GetReviewStatusDictionary()
                          where item.Key != "0"
                          select new SelectListItem
                          {
                              Text = item.Value,
                              Value = item.Key,
                              Selected = Equals(item.Key, status),
                          }).ToList();

            return statusList;
        }

        //Bind list of Review Ratings.
        public static List<SelectListItem> GetReviewRatings(int rating)
        {
            List<SelectListItem> ratingList = new List<SelectListItem>();

            ratingList = (from item in GetReviewRatingsDictionary()
                          select new SelectListItem
                          {
                              Text = item.Value,
                              Value = item.Key.ToString(),
                              Selected = Equals(item.Key, rating),
                          }).ToList();

            return ratingList;
        }

        //This method maps CustomerReviewListModel to CustomerReviewListViewModel.
        public static CustomerReviewListViewModel ToListViewModel(CustomerReviewListModel listModel)
        {
            if (listModel?.CustomerReviewList?.Count > 0)
            {
                return new CustomerReviewListViewModel()
                {
                    CustomerReviewList = (from _item in listModel.CustomerReviewList
                                          select new CustomerReviewViewModel
                                          {
                                              CMSCustomerReviewId = _item.CMSCustomerReviewId,
                                              PublishProductId = _item.PublishProductId,
                                              UserId = _item.UserId,
                                              Rating = _item.Rating,
                                              ProductName = _item.ProductName,
                                              Headline = _item.Headline,
                                              Comments = _item.Comments,
                                              UserName = _item.UserName,
                                              UserLocation = _item.UserLocation,
                                              Status = (GetReviewStatusDictionary().ContainsKey(_item.Status)) ? GetReviewStatusDictionary()[_item.Status] : _item.Status,
                                              CreatedDate = _item.CreatedDate.ToDateTimeFormat(),
                                              StoreName = _item.StoreName,
                                          }).ToList(),
                    Page = Convert.ToInt32(listModel.PageIndex),
                    RecordPerPage = Convert.ToInt32(listModel.PageSize),
                    TotalPages = Convert.ToInt32(listModel.TotalPages),
                    TotalResults = Convert.ToInt32(listModel.TotalResults)
                };
            }
            return null;
        }

        #region ReviewStatusDictionary
        //Dictionary for Review Status.
        public static Dictionary<string, string> GetReviewStatusDictionary()
        {
            Dictionary<string, string> reviewStatus = new Dictionary<string, string>();
            reviewStatus.Add("0", "All");
            reviewStatus.Add("A", "Active");
            reviewStatus.Add("I", "Inactive");
            reviewStatus.Add("N", "New");
            return reviewStatus;
        }
        #endregion

        #region ReviewRatingsDictionary
        //Dictionary for Review Ratings.
        public static Dictionary<int, string> GetReviewRatingsDictionary()
        {
            Dictionary<int, string> reviewRatings = new Dictionary<int, string>();
            reviewRatings.Add(5, "5 - Excellent, Perfect");
            reviewRatings.Add(4, "4 - That's Good Stuff");
            reviewRatings.Add(3, "3 - Average, Ordinary");
            reviewRatings.Add(2, "2 - Needs that Special Something");
            reviewRatings.Add(1, "1 - Not Good");
            return reviewRatings;
        }
        #endregion
    }
}