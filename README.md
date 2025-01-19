# Payment Services - Stripe Integration

This project provides an integration for handling payments using the Stripe API. It includes utilities and services that help manage and process payments securely and effectively. The primary focus of the project is to provide a robust error-handling mechanism for various payment-related errors using Stripe.

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
  - [StripeExceptionHandler](#stripeexceptionhandler)
- [Error Handling](#error-handling)
- [Testing](#testing)

## Overview

This project is a part of the `StripeNetSample` infrastructure, specifically designed to manage payments via Stripe. It includes:

- **Error handling** for Stripe-related exceptions.
- **Utility classes** to parse and format error messages for end users.
- **Support for multiple payment types** (e.g., credit cards, bank accounts).

The `StripeExceptionHandler` class is central to this project, offering user-friendly messages based on the Stripe exception codes.

## Installation

To use this project, follow these steps:

1. Clone the repository:

    ```bash
    git clone [https://github.com/Mohammad-Yasfo/StripeNetSample.git](https://github.com/Mohammad-Yasfo/StripeNetSample.git)
    ```

2. Install the required dependencies:

    This project relies on the Stripe .NET SDK. Install it via NuGet:

    ```bash
    dotnet add package Stripe.net --version x.x.x
    ```

   Replace `x.x.x` with the appropriate version of the Stripe SDK.

3. Build the project:

    ```bash
    dotnet build
    ```

## Configuration

1. **Stripe API Keys**: 

   Make sure to set up your Stripe API keys in your application's configuration file (e.g., `appsettings.json`) or environment variables.

   ```json
   {
     "StripeConfiguration": {
       "SecretKey": "your-secret-key",
       "PublishableKey": "your-publishable-key",
       "ClientId": "your_client_id",
       "OAuthBaseLink": "https://dashboard.stripe.com/oauth/authorize"
     }
   }
