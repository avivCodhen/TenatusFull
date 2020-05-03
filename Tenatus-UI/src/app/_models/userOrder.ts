export interface UserOrder {
  externalId: string;
  created: string;
  quantity: number;
  buyingPrice: number;
  userOrderActionType: string;
  orderAction: string;
  stock: string;
  userOrderType: string;
}
