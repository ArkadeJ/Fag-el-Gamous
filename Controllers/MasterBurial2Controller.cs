﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fag_el_Gamous.Models;
using Fag_el_Gamous.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Fag_el_Gamous.Models.Filtering;

namespace Fag_el_Gamous
{
    public class MasterBurial2Controller : Controller
    {
        private readonly waterbuffaloContext _context;

        public MasterBurial2Controller(waterbuffaloContext context)
        {
            _context = context;
        }

        // GET: MasterBurial2
        
        public async Task<IActionResult> Index(Filter filter, int? burialId, int pageNum = 1)
        {
            //creates a list of Carbons with Burial IDs (so we can link them in the view)
            ViewBag.CarbonBurialList = _context.Carbon2
                .Where(c => c.BurialId != null)
                .ToList();

             ViewBag.SampleBurialList = _context.Samples2
                .Where(c => c.BurialId != null)
                .ToList();


            ViewBag.PhotoBurialList = _context.Photos
                .Where(p => p.Burial != null)
                .ToList();

            //ViewBag.Sample = _context.Samples2
            //    .Where(c => c.BurialId != null)
            //    .ToList();

            //variable of type filterLogic
            var filterLogic = new FilterLogic(_context);

            //queryModel that receives the return from the GetMummies method
            var queryModel = filterLogic.GetMummies(filter);

            //Page size upped in order for the jQuery table to function properly
            int pageSize = 10000;

            int skip = 0;

            //Used for asp.net pagination
            if (pageNum - 1 < 0)
            { skip = 0; }
            else
            {
                skip = (pageNum - 1) * pageSize;
            }

            //ViewBag of the different people that have Authorization to Edit and Delete
            ViewBag.AdminList = _context.AspNetUsers
                .Where(b => b.isAdmin == true)
                .ToList();


            ViewBag.ResearchList = _context.AspNetUsers
                .Where(c => c.isResearcher == true)
                .ToList();

            ViewBag.CurrentUser = _context.AspNetUsers
                .Where(c => c.UserName == User.Identity.Name);

            //check to see if user is an admin
            var isAdmin = _context.AspNetUsers
                .Where(c => c.UserName == User.Identity.Name);

            foreach(var thing in isAdmin)
            {
                if( thing.isAdmin == true )
                {
                    ViewData["isAdmin"] = true;
                }
            }

            //check to see if the user is a researcher
            var isResearcher = _context.AspNetUsers
                .Where(c => c.UserName == User.Identity.Name);

            foreach (var thing in isResearcher)
            {
                if (thing.isResearcher == true)
                {
                    ViewData["isResearcher"] = true;
                }
            }

            


            //int b = 0;
            //foreach (var item in (_context.AspNetUsers.Where(c => c.isAdmin == true).ToList()))
            //{
            //    ViewData[$"UserName{b}"] = $"{item.UserName}";
            //    b = b + 1;
            //}



            return View(new PaginationViewModel
            {
                //Carbons = ((IQueryable<Carbon2>)_context.Carbon2.Include(c => c.Burial).ToListAsync()),

                //returning aspects of the ViewModel to be used in the view
                Burials = (queryModel
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList()),

                PageNumberingInfo = new PageNumberingInfo
                {
                    NumItemsPerPage = pageSize,
                    CurrentPage = pageNum,

                    TotalNumItems = (burialId == null ? queryModel.Count() :
                        queryModel.Where(x => x.BurialId == burialId).Count())
                },

                UrlInfo = Request.QueryString.Value
            });

        }

        // GET: MasterBurial2/Details/5
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            ViewBag.PhotoBurialList = _context.Photos
                .Where(p => p.Burial != null)
                .ToList();


            var masterBurial2 = await _context.MasterBurial2
                .FirstOrDefaultAsync(m => m.BurialId == id);
            if (masterBurial2 == null)
            {
                return NotFound();
            }

            ViewBag.Id = masterBurial2.BurialId;

            return View(masterBurial2);
        }

        // GET: MasterBurial2/Create
        [Authorize]
        public IActionResult Create()
        {
            var isAdmin = _context.AspNetUsers
                .Where(c => c.UserName == User.Identity.Name);

            //Check to see if the user is an admin
            foreach(var thing in isAdmin)
            {
                if( thing.isAdmin == true )
                {
                    ViewData["isAdmin"] = true;
                }
            }

            //check to see if the user is a researcher
            var isResearcher = _context.AspNetUsers
                .Where(c => c.UserName == User.Identity.Name);

            foreach (var thing in isResearcher)
            {
                if (thing.isResearcher == true)
                {
                    ViewData["isResearcher"] = true;
                }
            }

            return View();
        }

        // POST: MasterBurial2/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocConcat,BurialId,BurialLocationNs,LowPairNs,HighPairNs,BurialLocationEw,LowPairEw,HighPairEw,BurialSubplot,BurialNumber,BurialDepth,SouthToHead,SouthToFeet,WestToHead,WestToFeet,LengthOfRemains,BurialSituation,SampleNumber,GenderGe,GeFunctionTotal,GenderBodyCol,BasilarSuture,VentralArc,SubpubicAngle,SciaticNotch,PubicBone,PreaurSulcus,MedialIpRamus,DorsalPitting,ForemanMagnum,FemurHead,HumerusHead,Osteophytosis,PubicSymphysis,BoneLength,MedialClavicle,IliacCrest,FemurDiameter,Humerus,FemurLength,HumerusLength,TibiaLength,Robust,SupraorbitalRidges,OrbitEdge,ParietalBossing,Gonian,NuchalCrest,ZygomaticCrest,CranialSuture,MaximumCranialLength,MaximumCranialBreadth,BasionBregmaHeight,BasionNasion,BasionProsthionLength,BizygomaticDiameter,NasionProsthion,MaximumNasalBreadth,InterorbitalBreadth,ArtifactsDescription,HairColor,PreservationIndex,HairTakenTf,SoftTissueTakenTf,BoneTakenTf,ToothTakenTf,TextileTakenTf,ArtifactFoundTf,DescriptionOfTaken,EstimateAge,EstimateLivingStature,ToothAttrition,ToothEruption,PathologyAnomalies,EpiphysealUnion,YearFound,MonthFound,DayFound,HeadDirection,Preservation,Burialicon,Burialicon2,Sex,Sexmethod,Ageatdeath,Agemethod,Haircolor1,Sample,YearOnSkull,MonthOnSkull,DateOnSkull,FieldBook,FieldBookPageNumber,InitialsOfDataEntryExpert,InitialsOfDataEntryChecker,ByuSample,BodyAnalysis,SkullAtMagazine,PostcraniaAtMagazine,SexSkull,AgeSkull,RackAndShelf,ToBeConfirmed,SkullTrauma,PostcraniaTrauma,CribraOrbitala,PoroticHyperostosis,PoroticHyperostosisLocations,MetopicSuture,ButtonOsteoma,OsteologyUnknownComment,TemporalMandibularJointOsteoarthritisTmjOa,LinearHypoplasiaEnamel,AreaHillBurials,Tomb,BurialSubNumber,YearExcav,MonthExcavated,DateExcavated,BurialDirection,BurialPreservation,BurialWrapping,BurialAdultChild,GenderCode,Burialgendermethod,AgeCodeSingle,Burialageatdeath,Burialagemethod,HairColorCode,Burialhaircolor,Burialsampletaken,LengthM,LengthCm,Goods,Clstr,FaceBundle,OsteologyNotes")] MasterBurial2 masterBurial2)
        {
            if (ModelState.IsValid)
            {
                masterBurial2.LocConcat = ($"{masterBurial2.BurialLocationNs} {masterBurial2.LowPairNs} / {masterBurial2.HighPairNs} {masterBurial2.BurialLocationEw} {masterBurial2.LowPairEw} / {masterBurial2.HighPairEw} {masterBurial2.BurialSubplot} #{masterBurial2.BurialNumber}");


                _context.Add(masterBurial2);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(masterBurial2);
        }

        // GET: MasterBurial2/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            //Check to see is user is admin
            var isAdmin = _context.AspNetUsers
                .Where(c => c.UserName == User.Identity.Name);

            foreach(var thing in isAdmin)
            {
                if( thing.isAdmin == true )
                {
                    ViewData["isAdmin"] = true;
                }
            }

            //check to see if user is researcher
            var isResearcher = _context.AspNetUsers
                .Where(c => c.UserName == User.Identity.Name);

            foreach (var thing in isResearcher)
            {
                if (thing.isResearcher == true)
                {
                    ViewData["isResearcher"] = true;
                }
            }

            if (id == null)
            {
                return NotFound();
            }

            var masterBurial2 = await _context.MasterBurial2.FindAsync(id);
            if (masterBurial2 == null)
            {
                return NotFound();
            }
            return View(masterBurial2);
        }

        // POST: MasterBurial2/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocConcat,BurialId,BurialLocationNs,LowPairNs,HighPairNs,BurialLocationEw,LowPairEw,HighPairEw,BurialSubplot,BurialNumber,BurialDepth,SouthToHead,SouthToFeet,WestToHead,WestToFeet,LengthOfRemains,BurialSituation,SampleNumber,GenderGe,GeFunctionTotal,GenderBodyCol,BasilarSuture,VentralArc,SubpubicAngle,SciaticNotch,PubicBone,PreaurSulcus,MedialIpRamus,DorsalPitting,ForemanMagnum,FemurHead,HumerusHead,Osteophytosis,PubicSymphysis,BoneLength,MedialClavicle,IliacCrest,FemurDiameter,Humerus,FemurLength,HumerusLength,TibiaLength,Robust,SupraorbitalRidges,OrbitEdge,ParietalBossing,Gonian,NuchalCrest,ZygomaticCrest,CranialSuture,MaximumCranialLength,MaximumCranialBreadth,BasionBregmaHeight,BasionNasion,BasionProsthionLength,BizygomaticDiameter,NasionProsthion,MaximumNasalBreadth,InterorbitalBreadth,ArtifactsDescription,HairColor,PreservationIndex,HairTakenTf,SoftTissueTakenTf,BoneTakenTf,ToothTakenTf,TextileTakenTf,ArtifactFoundTf,DescriptionOfTaken,EstimateAge,EstimateLivingStature,ToothAttrition,ToothEruption,PathologyAnomalies,EpiphysealUnion,YearFound,MonthFound,DayFound,HeadDirection,Preservation,Burialicon,Burialicon2,Sex,Sexmethod,Ageatdeath,Agemethod,Haircolor1,Sample,YearOnSkull,MonthOnSkull,DateOnSkull,FieldBook,FieldBookPageNumber,InitialsOfDataEntryExpert,InitialsOfDataEntryChecker,ByuSample,BodyAnalysis,SkullAtMagazine,PostcraniaAtMagazine,SexSkull,AgeSkull,RackAndShelf,ToBeConfirmed,SkullTrauma,PostcraniaTrauma,CribraOrbitala,PoroticHyperostosis,PoroticHyperostosisLocations,MetopicSuture,ButtonOsteoma,OsteologyUnknownComment,TemporalMandibularJointOsteoarthritisTmjOa,LinearHypoplasiaEnamel,AreaHillBurials,Tomb,BurialSubNumber,YearExcav,MonthExcavated,DateExcavated,BurialDirection,BurialPreservation,BurialWrapping,BurialAdultChild,GenderCode,Burialgendermethod,AgeCodeSingle,Burialageatdeath,Burialagemethod,HairColorCode,Burialhaircolor,Burialsampletaken,LengthM,LengthCm,Goods,Clstr,FaceBundle,OsteologyNotes")] MasterBurial2 masterBurial2)
        {
            if (id != masterBurial2.BurialId)
            {
                
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //injects the correctly made LocConcat
                masterBurial2.LocConcat = ($"{masterBurial2.BurialLocationNs} {masterBurial2.LowPairNs} / {masterBurial2.HighPairNs} {masterBurial2.BurialLocationEw} {masterBurial2.LowPairEw} / {masterBurial2.HighPairEw} {masterBurial2.BurialSubplot} #{masterBurial2.BurialNumber}");


                try
                {
                    _context.Update(masterBurial2);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MasterBurial2Exists(masterBurial2.BurialId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(masterBurial2);
        }

        // GET: MasterBurial2/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            //Check to see if user is admin
            var isAdmin = _context.AspNetUsers
                .Where(c => c.UserName == User.Identity.Name);

            foreach(var thing in isAdmin)
            {
                if( thing.isAdmin == true )
                {
                    ViewData["isAdmin"] = true;
                }
            }

            if (id == null)
            {
                return NotFound();
            }

            var masterBurial2 = await _context.MasterBurial2
                .FirstOrDefaultAsync(m => m.BurialId == id);
            if (masterBurial2 == null)
            {
                return NotFound();
            }

            return View(masterBurial2);
        }

        // POST: MasterBurial2/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var masterBurial2 = await _context.MasterBurial2.FindAsync(id);
            _context.MasterBurial2.Remove(masterBurial2);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MasterBurial2Exists(int id)
        {
            return _context.MasterBurial2.Any(e => e.BurialId == id);
        }
    }
}
