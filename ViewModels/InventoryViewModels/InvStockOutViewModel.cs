using POS_APP.ViewModels.InventoryViewModels;

namespace POS_APP.ViewModels.InventoryViewModels
{
    class InvStockOutViewModel
    {
        private InventoryViewModel inventoryViewModel;

        public InvStockOutViewModel(InventoryViewModel inventoryViewModel)
        {
            this.inventoryViewModel = inventoryViewModel;
        }
    }
}
