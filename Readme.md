# Brendan's Payment Gateway

This is an API to act as a payment gateway, sending payments to the bank to be processed and allowing retrieval of previous payment details.

## Running

Build the solution and run the payment gateway project in your ide (easier) or run it manually from the command line from inside the solution folder with:


```bash
dotnet run --project=PaymentGateway
```
If swagger doesn't automatically open grab the first port the output listens on and use that.

```bash
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7240 <--- This
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5064
```

You can then navigate to [https://localhost:7240/swagger/index.html](https://localhost:7240/swagger/index.html) , make sure to substitute in your port.
Easiest way to run the APIs will be to use the tools provided on this site but I'll add some alternative methods to this as well.

### Curl

To send a payment use the following command, for testing purposes change the CVV to an odd number to receive a failed payment response. Again for this and the below command make sure to substitute your port:

```bash
curl -k -X 'POST' \
  'https://localhost:7240/api/Payment' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "cardNumber": "378282246310005",
  "cardExpiry": "12/2023",
  "cvv": "000",
  "amount": 123.1,
  "currency": "USD"
}'
```

To get a payment with an id of '124'. To fail to find a payment change the Id to an odd number.
```bash
curl -k -X 'GET' \
  'https://localhost:7240/api/Payment/124' \
  -H 'accept: text/plain'
```
# Design and implementation
## Design
I started with the basic requirements, listing out the basic inputs and outputs of each API then sketching out a very rough class structure. With the time limitation of 3 hours I decided on fairly minimal approaches to everything. 

I used ASP.NET to bootstrap an API with swagger for easy testing and API documentation as it seemed like a quick clean way to get a bunch of functionality for very little effort. It had been something I'd wanted to try using for a while and this was a good excuse.
 
I decided to just limit the bank simulator to a class for now, the quickest and simplest solution as I felt the API itself was worth more effort. 

## Implementation
I very quickly ran into the issue of wanting to write enterprise level code and having to do it all very quickly with no spec. I had to decide on key data structures on a whim despite this being a key part of building a good system. This meant I couldn't delve deeply on what might define a card, a payment amount, a payment request, a banks external payment response etc, let alone what values should be in each field. 

Similarly I didn't focus too much on project structure, I did some light commenting but couldn't tackle everything, I ended up not handling errors beyond the truly basic 404 scenario. Naming conventions were also something that suffered as a result of sub optimal class structures.

## Improvements and real world design
Real world I would have designed this cloud first, APIGateway and Lambda or the azure equivalent, with some networking (NAT or Internet Gateway) to access the banks API. So this would have used a slightly different structure on the Lambda itself, additionally IAC, CI/CD, Alerting etc. There would have been consideration on how to make this scalable across multiple different banks.

I would have started with a strong focus on core objects, Card, Money etc. Defining all these core concepts in a common library. Then some thought on core API conventions associated with these. Define clear models, then have builders to provide a cleaner interface for constructing these.

Auditing, Logging & clear validation failures and error messages would have been particularly key. Additionally Authorization.

Testing, I would have extended to negative unit testing, testing the API itself (rather than just the class) and testing it with the bank itself.

Also I would have made it more portable so that it could be run easier by you guys :D