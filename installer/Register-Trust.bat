@echo off
title Registrasi Keamanan PIEW Desktop
echo ========================================================
echo Mendaftarkan Sertifikat Resmi PIEW Software Foundation
echo ========================================================
echo.
cd /d "%~dp0"
if exist "PIEW-Software.cer" (
    certutil -addstore -user TrustedPublisher PIEW-Software.cer >nul 2>&1
    certutil -addstore -user Root PIEW-Software.cer >nul 2>&1
    echo [SUKSES] Sertifikat berhasil didaftarkan ke Windows!
    echo Windows sekarang mengenali PIEW Desktop sebagai publisher resmi yang aman.
) else (
    echo [PERINGATAN] File PIEW-Software.cer tidak ditemukan di folder ini.
)
echo.
echo ========================================================
pause
