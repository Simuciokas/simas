# Asgard Marketplace

## Prerequisites

- .NET 6 SDK [download](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## How To Run

- Install dependencies (`dotnet restore`)
- Build the app (`dotnet build`)
- Run the application (`dotnet run -p AsgardMarketplace.Api`)
- Navigate to `https://localhost:5001/swagger` to see if it works

## How it works

Asgard Marketplace is a simple marketplace that allows seller to create items for sale and buyers to buy those items.

Main functionality of the app:

- Item can be created by the seller. Maximum quantity for item is 10;
- Item can be retrieved by itemId;
- Item can be deleted by itemId;
- Buyer can purchase item. Purchasing an item creates and order in Unpaid state;
- System can mark order as paid;
- All orders created by buyer can be retrieved;
- Order can be marked as delivered by seller
