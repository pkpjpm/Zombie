/*
 *               ZOMBIE utility library for QBFC
 *
 *             created by Paul Keister (pk@pjpm.biz)
 *                copyright (c) 2003 - 2012 PJPM
 *
 *  Licensed under the Eclipse Public License 1.0 (EPL-1.0)
 *  full license available at http://opensource.org/licenses/EPL-1.0
 */

using Interop.QBFC13;

namespace Zombie
{
  /// <summary>
  /// The ItemDisambiguator is used when running through an item list. Specify
  /// actions to be taken only on item types that interest you by using
  /// type-safe delegates. This eliminates quite a bit of ceremony code
  /// that would otherwise be needed to figure out what type of item you're
  /// dealing with.
  /// </summary>
  public class ItemDisambiguator
  {
    public delegate void DiscountCallback(IItemDiscountRet itm);

    public DiscountCallback IfDiscountItem { get; set; }

    public delegate void FixedAssetCallback(IItemFixedAssetRet itm);

    public FixedAssetCallback IfFixedAsset { get; set; }

    public delegate void GroupCallback(IItemGroupRet itm);

    public GroupCallback IfGroup { get; set; }

    public delegate void InventoryAssemblyCallback(IItemInventoryAssemblyRet itm);

    public InventoryAssemblyCallback IfInventoryAssembly { get; set; }

    public delegate void InventoryCallback(IItemInventoryRet itm);

    public InventoryCallback IfInventory { get; set; }

    public delegate void NonInventoryCallback(IItemNonInventoryRet itm);

    public NonInventoryCallback IfNonIventory { get; set; }

    public delegate void OtherChargeCallback(IItemOtherChargeRet itm);

    public OtherChargeCallback IfOtherCharge { get; set; }

    public delegate void PaymentCallback(IItemPaymentRet itm);

    public PaymentCallback IfPayment { get; set; }

    public delegate void SalesTaxGroupCallback(IItemSalesTaxGroupRet itm);

    public SalesTaxGroupCallback IfSalesTaxGroup { get; set; }

    public delegate void SalesTaxCallback(IItemSalesTaxRet itm);

    public SalesTaxCallback IfSalesTax { get; set; }

    public delegate void ServiceCallback(IItemServiceRet itm);

    public ServiceCallback IfService { get; set; }

    public delegate void SubTotalCallback(IItemSubtotalRet itm);

    public SubTotalCallback IfSubtotal { get; set; }

    /// <summary>
    /// The Process method calls the appropriate delegate based on the type of
    /// the item passed in.
    /// </summary>
    /// <param name="item">The raw item reference</param>
    /// <returns>Returns true if the item was processed by a callback, otherwise false</returns>
    public bool Process(IORItemRet item)
    {
      if (item.ItemDiscountRet != null && IfDiscountItem != null)
      {
        IfDiscountItem(item.ItemDiscountRet);

        return true;
      }
      if (item.ItemFixedAssetRet != null && IfFixedAsset != null)
      {
        IfFixedAsset(item.ItemFixedAssetRet);

        return true;
      }
      if (item.ItemGroupRet != null && IfGroup != null)
      {
        IfGroup(item.ItemGroupRet);

        return true;
      }
      if (item.ItemInventoryAssemblyRet != null && IfInventoryAssembly != null)
      {
        IfInventoryAssembly(item.ItemInventoryAssemblyRet);

        return true;
      }
      if (item.ItemInventoryRet != null && IfInventory != null)
      {
        IfInventory(item.ItemInventoryRet);

        return true;
      }
      if (item.ItemNonInventoryRet != null && IfNonIventory != null)
      {
        IfNonIventory(item.ItemNonInventoryRet);

        return true;
      }
      if (item.ItemOtherChargeRet != null && IfOtherCharge != null)
      {
        IfOtherCharge(item.ItemOtherChargeRet);

        return true;
      }
      if (item.ItemPaymentRet != null && IfPayment != null)
      {
        IfPayment(item.ItemPaymentRet);

        return true;
      }
      if (item.ItemSalesTaxGroupRet != null && IfSalesTaxGroup != null)
      {
        IfSalesTaxGroup(item.ItemSalesTaxGroupRet);

        return true;
      }
      if (item.ItemSalesTaxRet != null && IfSalesTax != null)
      {
        IfSalesTax(item.ItemSalesTaxRet);

        return true;
      }
      if (item.ItemServiceRet != null && IfService != null)
      {
        IfService(item.ItemServiceRet);

        return true;
      }
      if (item.ItemSubtotalRet != null && IfSubtotal != null)
      {
        IfSubtotal(item.ItemSubtotalRet);

        return true;
      }

      return false;
    }
  }
}