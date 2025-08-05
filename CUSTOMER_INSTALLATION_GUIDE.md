# VillageSmart POS - Customer Installation Guide

## 🎯 **Quick Start Installation**

### **Step 1: System Requirements Check**
Before installing, ensure your computer meets these requirements:

**Minimum Requirements:**
- ✅ Windows 10 (1903+) or Windows 11 (64-bit)
- ✅ 4 GB RAM (8 GB recommended)
- ✅ 500 MB free disk space
- ✅ Internet connection for initial setup

**Recommended Requirements:**
- ✅ Windows 11
- ✅ Intel Core i5 or AMD equivalent (2.5 GHz+)
- ✅ 8 GB RAM
- ✅ 1 GB free disk space
- ✅ 1920x1080 display resolution

### **Step 2: Download and Extract**
1. **Download the ZIP file:** `VillageSmartPOS-v1.0.0.zip`
2. **Extract to:** `C:\VillageSmartPOS\`
3. **Verify files:** You should see:
   - `VillageSmartPOS.exe` (Main application)
   - `README_Installation.md` (This guide)
   - `DatabaseMigration.sql` (Database setup)
   - `DatabaseMigration_BillSettings.sql`
   - `DatabaseMigration_LoanCustomers.sql`

### **Step 3: Install Dependencies**

#### **A. .NET 8.0 Desktop Runtime**
1. **Download:** https://dotnet.microsoft.com/download/dotnet/8.0
2. **Choose:** "Desktop Runtime 8.0.x" for Windows
3. **Install:** Run the downloaded `.exe` file
4. **Restart:** Computer if prompted

#### **B. MySQL Server 8.0**
1. **Download:** https://dev.mysql.com/downloads/mysql/
2. **Choose:** "MySQL Installer for Windows"
3. **Install:** Run with default settings
4. **Set Password:** Remember your root password!
5. **Start Service:** Ensure MySQL service is running

### **Step 4: Database Setup**

#### **A. Create Database**
1. **Open MySQL Workbench** or Command Line
2. **Connect** to MySQL server
3. **Run these commands:**
   ```sql
   CREATE DATABASE villagesmartpos;
   USE villagesmartpos;
   ```

#### **B. Run Migration Scripts**
1. **Open MySQL Workbench**
2. **Open each file** in this order:
   - `DatabaseMigration.sql`
   - `DatabaseMigration_BillSettings.sql`
   - `DatabaseMigration_LoanCustomers.sql`
3. **Execute** each script

### **Step 5: Configure Application**

#### **A. First Launch**
1. **Run:** `VillageSmartPOS.exe`
2. **Configure Database Connection:**
   - Server: `localhost`
   - Port: `3306`
   - Database: `villagesmartpos`
   - Username: `root`
   - Password: `[your MySQL password]`

#### **B. Store Settings**
1. **Go to:** Settings page
2. **Configure:**
   - Store name and address
   - Contact information
   - Receipt customization
   - Currency settings (Rs.)

### **Step 6: Hardware Setup**

#### **A. Thermal Printer (80mm)**
1. **Install Drivers:** Use manufacturer drivers
2. **Connect:** USB or network connection
3. **Set Default:** Make it the default printer
4. **Test Print:** Print a test page
5. **Configure in POS:** Settings → Printer Settings

#### **B. Barcode Scanner (Optional)**
1. **Connect:** USB scanner
2. **Install Drivers:** If needed
3. **Test Scanning:** Scan a barcode
4. **Configure:** Set to USB HID mode

### **Step 7: Testing**

#### **A. Basic Functions Test**
- [ ] **Add Products:** Add a few test products
- [ ] **Sales Transaction:** Create a test sale
- [ ] **Print Receipt:** Test receipt printing
- [ ] **Inventory Management:** Check stock levels

#### **B. Advanced Features Test**
- [ ] **Loan Customers:** Add test customers
- [ ] **Settings:** Configure store settings
- [ ] **Backup:** Test backup functionality

## 🔧 **Troubleshooting**

### **Common Issues & Solutions:**

#### **1. Application Won't Start**
**Symptoms:** Double-clicking exe does nothing
**Solutions:**
- Check .NET 8.0 runtime installation
- Run as Administrator
- Check antivirus exclusions
- Verify Windows compatibility

#### **2. Database Connection Error**
**Symptoms:** "Cannot connect to database"
**Solutions:**
- Verify MySQL service is running
- Check connection string
- Test network connectivity
- Verify database exists

#### **3. Printer Not Working**
**Symptoms:** Receipts don't print
**Solutions:**
- Check printer drivers
- Verify default printer setting
- Test print functionality
- Check printer connection

#### **4. Performance Issues**
**Symptoms:** Slow response, lag
**Solutions:**
- Check system resources
- Close unnecessary applications
- Update graphics drivers
- Restart application

## 📞 **Support Information**

### **Contact Details:**
- **Email:** support@villagesmartpos.com
- **Phone:** +94-11-123-4567
- **Website:** www.villagesmartpos.com

### **Documentation:**
- **User Manual:** Available in application
- **Admin Guide:** Contact support
- **Video Tutorials:** Check website

## 🔄 **Updates & Maintenance**

### **Automatic Updates:**
- Application checks for updates on startup
- Download and install automatically
- Backup data before updates

### **Manual Updates:**
1. Download latest release
2. Stop application
3. Replace executable files
4. Restart application

## 📊 **System Monitoring**

### **Performance Monitoring:**
- Monitor CPU and memory usage
- Check disk space regularly
- Monitor database performance
- Track application logs

### **Backup Strategy:**
- Daily database backups
- Weekly full system backups
- Store backups securely
- Test restore procedures

## 🎉 **Congratulations!**

Your VillageSmart POS system is now ready for use!

### **Next Steps:**
1. **Train Staff:** Show them how to use the system
2. **Add Products:** Import your product catalog
3. **Configure Settings:** Customize for your business
4. **Test Everything:** Run through all features
5. **Go Live:** Start using for real transactions

### **Tips for Success:**
- **Start Small:** Begin with basic sales
- **Train Gradually:** Introduce features one by one
- **Backup Regularly:** Keep data safe
- **Monitor Performance:** Watch for issues
- **Get Support:** Don't hesitate to ask for help

---

**Version:** 1.0.0  
**Last Updated:** December 2024  
**Support:** VillageSmart POS Team

**Need Help?** Contact us anytime at support@villagesmartpos.com 