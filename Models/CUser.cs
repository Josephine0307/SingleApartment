﻿using sln_SingleApartment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sln_SingleApartment.Models
{
    public class CUser
    {
        private SingleApartmentEntities db = new SingleApartmentEntities();
        public tMember tMember { get; set; }
        
        #region 商城組

        #region 我的最愛
        //查詢我的最愛
        public List<CFavoriteList> SearchFavorite()
        {
            List<CFavoriteList> list = new List<CFavoriteList>();
            var fav = db.FavoriteList.Where(r => r.MemberID == tMember.fMemberId);
            foreach (var item in fav)
            {
                CFavoriteList Memberfav = new CFavoriteList() { entity = item };
                list.Add(Memberfav);
            }
            return list;
        }
        //將產品加到我的最愛
        public string AddToFavorite(int ProductID)
        {
            var pdName = db.Product.Where(r => r.ProductID == ProductID).FirstOrDefault().ProductName;
            var fa = db.FavoriteList.Where(r => r.MemberID == tMember.fMemberId && r.ProductID == ProductID);
            if (fa.Count() == 0)
            {
                try
                {
                    FavoriteList fv = new FavoriteList() { MemberID = tMember.fMemberId, ProductID = ProductID };
                    db.FavoriteList.Add(fv);
                    db.SaveChanges();
                    return $"已成功將 {pdName} 加入我的最愛";
                }
                catch (Exception)
                {
                    return $"出現錯誤！請稍後再嘗試！";
                }
            }
            else return "我的最愛裡已有此件商品";

        }
        //刪除我的最愛
        public string DeleteFavorite(int ProductID)
        {
            var fa = db.FavoriteList.Where(r => r.MemberID == tMember.fMemberId && r.ProductID == ProductID).FirstOrDefault();
            if (fa != null)
            {
                db.FavoriteList.Remove(fa);
                db.SaveChanges();
                return "刪除成功";
            }
            return "我的最愛裡沒有此商品，請再試一次！";
        }
        #endregion
        #region 購物車
        //查看購物車裡的商品
        public List<COrderDetailsViewModel> SearchProductInCart(List<CAddtoSessionView> list)
        {
            List<COrderDetailsViewModel> orderDetails = new List<COrderDetailsViewModel>();
            foreach (var item in list)
            {
                COrderDetailsViewModel odd = new COrderDetailsViewModel();
                odd.entity = new OrderDetails() { ProductID = item.txtProductID, Quantity = item.txtQuantity };
                orderDetails.Add(odd);
            }
            return orderDetails;
        }
        #endregion
        #region 商品
        //查詢商品
        public ShopViewModel SearchProduct()
        {
            List<CProductViewModel> list_product = new List<CProductViewModel>();
            foreach (var item in db.Product.Where(r => r.Discontinued == "N" && r.Stock >= 0 && r.ActivityID==null))
            {
                list_product.Add(new CProductViewModel() { entity = item });
            }

            List<CProductMainCategoryViewModel> list_main = new List<CProductMainCategoryViewModel>();
            foreach (var item in db.ProductMainCategory)
            {
                list_main.Add(new CProductMainCategoryViewModel()
                {
                    entity_MainCategory = item,
                    ProductCount = list_product.Where(r => r.MainCategoryID == item.ProductMainCategoryID).Count()
                });
            }
            List<CProductSubCategoryViewModel> list_sub = new List<CProductSubCategoryViewModel>();
            foreach (var item in db.ProductSubCategory)
            {
                list_sub.Add(new CProductSubCategoryViewModel()
                {
                    entity_SubCategory = item,
                    ProductCount = list_product.Where(r => r.SubCategoryID == item.ProductSubCategoryID).Count()
                });
            }
            
            ShopViewModel result = new ShopViewModel() { product = list_product, MainCategory = list_main, SubCategory = list_sub };
            return result;
        }
        public List<CProductViewModel> SearchProductsBy(int? MainCategory=null, int? SubCategory=null, string KeyWord="")
        {
            List<CProductViewModel> result = new List<CProductViewModel>();
            var pd = db.Product.Where(r => r.Discontinued == "N" && r.Stock >= 0 && r.ActivityID == null);
            if (!String.IsNullOrEmpty(KeyWord))
            {
                pd = db.Product.Where(r => r.ProductName.Contains(KeyWord) && r.Discontinued == "N" && r.Stock >= 0 && r.ActivityID == null);
            }
            else if (SubCategory!= null)
            {
                pd = db.Product.Where(r => r.ProductSubCategoryID == SubCategory && r.Discontinued == "N" && r.Stock >= 0 && r.ActivityID == null);
            }
            else if (MainCategory!= null)
            {
                pd = db.Product.Where(r => r.ProductSubCategory.ProductMainCategoryID == MainCategory && r.Discontinued == "N" && r.Stock >= 0 && r.ActivityID == null);
            }
            foreach(var item in pd)
            {
                result.Add(new CProductViewModel() { entity = item });
            }
            return result;
        }


        #endregion

        #endregion

        //將房間加到我的最愛
        public string AddRoomToFavorite(int RoomID)
        {
            var roomName = db.Room.Where(r => r.ID == RoomID).FirstOrDefault().RoomName;
            var roomfa = db.RoomFavorite.Where(r => r.MemberID == tMember.fMemberId && r.RoomID == RoomID);
            if (roomfa.Count() == 0)
            {
                try
                {
                    RoomFavorite roomfv = new RoomFavorite() { MemberID = tMember.fMemberId, RoomID = RoomID };
                    db.RoomFavorite.Add(roomfv);
                    db.SaveChanges();

                    //50010 加入我的最愛
                    CInformationFactory x = new CInformationFactory();
                    x.Add(tMember.fMemberId, 400, 0, 40010);

                    return $"已成功將 {roomName} 加入我的最愛";
                }
                catch (Exception)
                {
                    return $"出現錯誤！請稍後再嘗試！";
                }
            }
            else return "我的最愛裡已有此房間";

        }

        //查詢房間的我的最愛
        public List<CRoomFavorite> SearchRoomFavorite()
        {
            List<CRoomFavorite> list = new List<CRoomFavorite>();
            var romfav = db.RoomFavorite.Where(r => r.MemberID == tMember.fMemberId);
            foreach (var item in romfav)
            {
                CRoomFavorite MemberRoomfav = new CRoomFavorite() { entity_RoomFavorite = item };
                list.Add(MemberRoomfav);
            }
            return list;
        }

        //刪除房間的我的最愛
        public string DeleteRoomFavorite(int RoomID)
        {
            var roomfa = db.RoomFavorite.Where(r => r.MemberID == tMember.fMemberId && r.RoomID == RoomID).FirstOrDefault();
            if (roomfa != null)
            {
                db.RoomFavorite.Remove(roomfa);
                db.SaveChanges();
                return "刪除成功";
            }
            return "我的最愛裡沒有此商品，請再試一次！";
        }


    }
}