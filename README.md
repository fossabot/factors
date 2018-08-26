# Factors
[![Build Status](https://travis-ci.org/bradmb/factors.svg?branch=master)](https://travis-ci.org/bradmb/factors)

*NETStandard 2.0 library that makes it easy to implement multi-factor (and FIDO2) authentication into your application*

Lots to still do on this project, but here's what is built out so far:
* Support for multiple database types (via ServiceStack.OrmLite)
* Users can have more than one credential (example: three phone numbers, FIDO2 key, and a TOTP token)
* Database one-way hashing of tokens
* Numerical multi-factor tokens
* Text-based multi-factor tokens
* Alphanumeric multi-factor tokens
* Modular so you only have to install the features you want
* Email multi-factor authentication (with support for SMTP and Postmark as mail providers)

To-do:
* Text message multi-factor (via Twilio)
* Phone call multi-factor (via Twilio)
* Authy multi-factor
* TOTP multi-factor
* HOTP multi-factor
* FIDO2 support (for WebAuthn)

----------

*Pull requests and issue reports always appreciated*
