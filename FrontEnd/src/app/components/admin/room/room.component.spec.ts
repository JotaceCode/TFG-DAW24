import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomTableComponent } from './room.component';

describe('RoomTableComponent', () => {
  let component: RoomTableComponent;
  let fixture: ComponentFixture<RoomTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoomTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoomTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
